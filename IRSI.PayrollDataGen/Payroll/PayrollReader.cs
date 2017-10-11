using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IRSI.PayrollDataGen.Payroll.AlohaDataTableAdapters;
using NLog;

namespace IRSI.PayrollDataGen.Payroll
{
  public class PayrollReader : IPayrollReader
  {
    private readonly ILogger _logger;

    public PayrollReader()
    {
      _logger = LogManager.GetCurrentClassLogger();
    }

    public AlohaData ReadPayroll(string datedFolder)
    {
      _logger.Info($"Read Payroll called for folder {datedFolder}");
      var connectionString = GetConnectionString(datedFolder);
      try
      {
        var data = new AlohaData();

        _logger.Debug($"Loading emp table");
        var empAdapter = new empTableAdapter();
        empAdapter.Connection.ConnectionString = connectionString;
        empAdapter.Fill(data.emp);

        _logger.Debug($"Loading adjtime table");
        var adjTimeAdapter = new adjtimeTableAdapter();
        adjTimeAdapter.Connection.ConnectionString = connectionString;
        adjTimeAdapter.Fill(data.adjtime);

        _logger.Debug($"Loading gndbreak table");
        var gndBreakAdapter = new gndbreakTableAdapter();
        gndBreakAdapter.Connection.ConnectionString = connectionString;
        gndBreakAdapter.Fill(data.gndbreak);

        _logger.Info($"Read Payroll finished");
        return data;
      } catch (Exception e)
      {
        _logger.Error(e, $"Error loading payroll tables");
      }

      return null;
    }

    private string GetConnectionString(string datedFolder)
    {
      return $"Provider=VFPOLEDB.1;Data Source={datedFolder}";
    }
  }
}
