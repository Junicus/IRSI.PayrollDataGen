using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IRSI.PayrollDataGen.Payroll.Model;
using IRSI.PayrollDataGen.Properties;
using NLog;

namespace IRSI.PayrollDataGen.Payroll
{
  public class PayrollConverter : IPayrollConverter
  {
    private ILogger _logger;

    public PayrollConverter()
    {
      _logger = LogManager.GetCurrentClassLogger();
    }

    public List<Employee> ConvertPayroll(AlohaData alohaData)
    {
      _logger.Info($"Convert Payroll called");
      var employeeDictionary = new Dictionary<int, Employee>();
      _logger.Debug($"Begin Loading Employees");
      foreach (var employee in alohaData.emp.AsEnumerable())
      {
        if (!employeeDictionary.Keys.Contains(employee.id))
          employeeDictionary.Add(employee.id, new Employee {
            ID = employee.id,
            SocialSecurity = employee.sec_num.TrimStart('0'),
            FirstName = employee.firstname.Trim(),
            LastName = employee.lastname.Trim(),
            Transactions = new Transactions()
          });
      }
      _logger.Debug($"End Loading Employees, {employeeDictionary.Count} employees loaded");

      _logger.Debug($"Begin loading shift");
      var adjTimeShift = alohaData.adjtime.AsEnumerable().Where(j => j.jobcode != 11).Select(t => {
        try
        {
          return new Transaction {
            Type = TransactionType.ClockInOut,
            PeriodType = PeriodType.Shift,
            EmpID = t.employee,
            SSN = t.ssn,
            JobCode = t.jobcode,
            Hours = (t.IshoursNull() ? 0 : t.hours),
            PayCode = string.Empty,
            ClockIn = t.sysdatein.AddHours(t.inhour).AddMinutes(t.inminute),
            ClockOut = t.sysdateout.AddHours(t.outhour).AddMinutes(t.outminute)
          };
        } catch (Exception e)
        {
          _logger.Error(e, $"Error loading shift adjtime: {e.Message}");
        }
        return null;
      });
      _logger.Debug($"End loading shift, {adjTimeShift.Count()} loaded");

      var tipCalculationStrategy = Settings.Default.TipCalculationStrategy;
      _logger.Debug($"Begin loading tips, using {tipCalculationStrategy}");
      var adjTimeTips = alohaData.adjtime.AsEnumerable().Where(j => j.jobcode != 11).Where(t => t.dectips > 0m || t.cctips > 0m)
          .Select(t => {
            try
            {
              return new Transaction {
                Type = TransactionType.PayCode,
                PeriodType = PeriodType.Unknown,
                EmpID = t.employee,
                SSN = t.ssn,
                JobCode = t.jobcode,
                Hours = 0m,
                PayCode = "T",
                Tips = (tipCalculationStrategy == TipsCalculation.Auto) ?
                  (t.dectips > t.cctips) ? t.dectips : t.cctips :
                  (tipCalculationStrategy == TipsCalculation.CCTips) ? t.cctips : t.dectips,
                ClockIn = t.date.AddHours(12)
              };
            } catch (Exception e)
            {
              _logger.Error(e, $"Error loading tips from adjtime: {e.Message}");
            }
            return null;
          });
      _logger.Debug($"End loading tips, {adjTimeTips.Count()} loaded");

      _logger.Debug($"Begin loading breaks");
      var breaks = alohaData.gndbreak.AsEnumerable().Where(j => j.jobcode != 11).Select(t => {
        try
        {
          return new Transaction {
            Type = TransactionType.ClockInOut,
            PeriodType = PeriodType.Break,
            EmpID = t.employee,
            SSN = t.ssn,
            JobCode = t.jobcode,
            Hours = (t.IshoursNull() ? 0 : t.hours),
            PayCode = string.Empty,
            ClockIn = t.sysdatebeg.AddHours(t.inhour).AddMinutes(t.inminute),
            ClockOut = t.sysdateend.AddHours(t.outhour).AddMinutes(t.outminute)
          };
        } catch (Exception e)
        {
          _logger.Error(e, $"Error loading breaks from gndbreak: {e.Message}");
        }
        return null;
      });
      _logger.Debug($"End loading breaks, {breaks.Count()} loaded");

      _logger.Debug($"Beging Assigning transactions to employees");
      foreach (var t in adjTimeShift)
      {
        if (t != null)
          if (employeeDictionary.Keys.Contains(t.EmpID))
          {
            t.SSN = employeeDictionary[t.EmpID].SocialSecurity;
            employeeDictionary[t.EmpID].Transactions.Add(t);
          } else
          {
            _logger.Warn($"Employee {t.EmpID} with SSN {t.SSN} not found");
          }
      }

      foreach (var t in adjTimeTips)
      {
        if (t != null)
          if (employeeDictionary.Keys.Contains(t.EmpID))
          {
            t.SSN = employeeDictionary[t.EmpID].SocialSecurity;
            employeeDictionary[t.EmpID].Transactions.Add(t);
          } else
          {
            _logger.Warn($"Employee {t.EmpID} with SSN {t.SSN} not found");
          }
      }

      foreach (var t in breaks)
      {
        if (t != null)
          if (employeeDictionary.Keys.Contains(t.EmpID))
          {
            t.SSN = employeeDictionary[t.EmpID].SocialSecurity;
            employeeDictionary[t.EmpID].Transactions.Add(t);
          } else
          {
            _logger.Warn($"Employee {t.EmpID} with SSN {t.SSN} not found");
          }
      }
      _logger.Debug($"End Assigning transactions to employees");

      _logger.Debug($"Beging adjusting paycodes for multiple shifts");
      foreach (var employee in employeeDictionary.Values.Where(t => t.Transactions.Any()))
      {
        var minutesToAdd = 0;
        foreach (var t in employee.Transactions.Where(t => t.Type == TransactionType.PayCode))
        {
          t.ClockIn = t.ClockIn.AddMinutes(minutesToAdd);
          minutesToAdd += 5;
        }
      }
      _logger.Debug($"End adjusting paycodes for multiple shifts");

      _logger.Info($"Convert Payroll finished");
      return employeeDictionary.Values.ToList<Employee>();
    }
  }
}
