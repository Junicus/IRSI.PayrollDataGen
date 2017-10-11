using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas;
using Autofac;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace IRSI.PayrollDataGen
{
  class Program
  {
    static void Main(string[] args)
    {
      SetupLogging();

      var configuration = Host.UseAppConfig<PayrollGenService>()
      .AllowMultipleInstances()
      .WithRegistrations(b => b.RegisterModule(new PayrollGenModule()))
      .WithArguments(args);
      Host.Start(configuration);
    }

    static void SetupLogging()
    {
      var config = new LoggingConfiguration();

      var consoleTarget = new ColoredConsoleTarget();
      config.AddTarget("console", consoleTarget);

      var fileTarget = new FileTarget();
      config.AddTarget("file", fileTarget);

      consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
      fileTarget.FileName = "${basedir}/logs/payrollGen.log";
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
