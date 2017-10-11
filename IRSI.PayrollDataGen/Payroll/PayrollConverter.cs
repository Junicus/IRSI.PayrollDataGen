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
      var employeeDictionary = new Dictionary<string, Employee>();
      _logger.Debug($"Begin Loading Employees");
      foreach (var employee in alohaData.emp.AsEnumerable())
      {
        if (!employeeDictionary.Keys.Contains(employee.ssn))
          employeeDictionary.Add(employee.ssn, new Employee {
            ID = employee.id,
            SocialSecurity = employee.ssn.TrimStart('0'),
            FirstName = employee.firstname.Trim(),
            LastName = employee.lastname.Trim(),
            Trasactions = new Transactions()
          });
      }
      _logger.Debug($"End Loading Employees, {employeeDictionary.Count} employees loaded");

      _logger.Debug($"Begin loading shift");
      var adjTimeShift = alohaData.adjtime.AsEnumerable().Select(t => {
        try
        {
          return new Transaction {
            Type = TransactionType.ClockInOut,
            PeriodType = PeriodType.Shift,
            EmpID = t.employee,
            SSN = t.ssn,
            JobCode = t.jobcode,
            Hours = (t.IshoursNull() ? 0 : t.hours),
            ClockIn = t.sysdatein.AddHours(t.inhour).AddMinutes(t.inminute),
            ClockOut = t.sysdateout.AddHours(t.inhour).AddMinutes(t.outminute)
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
      var adjTimeTips = alohaData.adjtime.AsEnumerable().Where(t => t.dectips > 0m || t.cctips > 0m)
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
      var breaks = alohaData.gndbreak.AsEnumerable().Select(t => {
        try
        {
          return new Transaction {
            Type = TransactionType.ClockInOut,
            PeriodType = PeriodType.Break,
            EmpID = t.employee,
            SSN = t.ssn,
            JobCode = t.jobcode,
            ClockIn = t.sysdatebeg.AddHours(t.inhour).AddMinutes(t.inminute),
            ClockOut = t.sysdateend.AddHours(t.inhour).AddMinutes(t.outminute)
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
          if (employeeDictionary.Keys.Contains(t.SSN))
          {
            employeeDictionary[t.SSN].Trasactions.Add(t);
          } else
          {
            _logger.Warn($"Employee {t.EmpID} with SSN {t.SSN} not found");
          }
      }

      foreach (var t in adjTimeTips)
      {
        if (t != null)
          if (employeeDictionary.Keys.Contains(t.SSN))
          {
            employeeDictionary[t.SSN].Trasactions.Add(t);
          } else
          {
            _logger.Warn($"Employee {t.EmpID} with SSN {t.SSN} not found");
          }
      }

      foreach (var t in breaks)
      {
        if (t != null)
          if (employeeDictionary.Keys.Contains(t.SSN))
          {
            employeeDictionary[t.SSN].Trasactions.Add(t);
          } else
          {
            _logger.Warn($"Employee {t.EmpID} with SSN {t.SSN} not found");
          }
      }
      _logger.Debug($"End Assigning transactions to employees");

      _logger.Debug($"Beging adjusting paycodes for multiple shifts");
      foreach (var employee in employeeDictionary.Values.Where(t => t.Trasactions.Any()))
      {
        var minutesToAdd = 0;
        foreach (var t in employee.Trasactions.Where(t => t.Type == TransactionType.PayCode))
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
