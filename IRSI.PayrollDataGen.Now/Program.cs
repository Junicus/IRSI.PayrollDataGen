using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using IRSI.PayrollDataGen.Now.Services;
using IRSI.PayrollDataGen.Payroll;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace IRSI.PayrollDataGen.Now
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      IPayrollReader PayrollReader = new PayrollReader();
      IPayrollConverter PayrollConverter = new PayrollConverter();
      IPayrollWriter PayrollWritter = new PayrollXmlWriter();

      IDatedFolderProvider DatedFolderProvider = new DatedFolderProvider();

      SetupLogging();

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new frmGeneratePayroll(PayrollReader, PayrollConverter, PayrollWritter, DatedFolderProvider));
    }

    static void SetupLogging()
    {
      var config = new LoggingConfiguration();

      var consoleTarget = new ColoredConsoleTarget();
      config.AddTarget("console", consoleTarget);

      var fileTarget = new FileTarget();
      config.AddTarget("file", fileTarget);

      consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
      fileTarget.FileName = "${basedir}/logs/payrollGenNow.log";
      fileTarget.ArchiveOldFileOnStartup = true;
      fileTarget.MaxArchiveFiles = 7;
      fileTarget.ArchiveEvery = FileArchivePeriod.Day;

      var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
      config.LoggingRules.Add(rule1);

      var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
      config.LoggingRules.Add(rule2);

      LogManager.Configuration = config;
    }

  }
}
