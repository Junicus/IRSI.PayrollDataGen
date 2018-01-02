using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRSI.PayrollDataGen.Ftp;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace IRSI.SendPayroll
{
  class Program
  {
    static void Main(string[] args)
    {
      IFtpSendPayroll FtpSendPayroll = new FtpSendPayroll();
      ILogger _logger = LogManager.GetCurrentClassLogger();

      var config = new LoggingConfiguration();

      var consoleTarget = new ColoredConsoleTarget();
      config.AddTarget("console", consoleTarget);

      var fileTarget = new FileTarget();
      config.AddTarget("file", fileTarget);

      consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
      fileTarget.FileName = "${basedir}/logs/sendpayroll.log";
      fileTarget.ArchiveOldFileOnStartup = true;
      fileTarget.MaxArchiveFiles = 7;
      fileTarget.ArchiveEvery = FileArchivePeriod.Day;

      LogManager.Configuration = config;

      _logger.Debug($"PayrollSendJob running at {DateTime.Now.ToString()}");

      FtpSendPayroll.RunJob();

      _logger.Debug($"PayrollSendJob finished at {DateTime.Now.ToString()}");
    }
  }
}
