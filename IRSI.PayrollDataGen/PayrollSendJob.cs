using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using IRSI.PayrollDataGen.Ftp;
using IRSI.PayrollDataGen.Ftp.Model;
using IRSI.PayrollDataGen.Properties;
using Newtonsoft.Json.Linq;
using NLog;
using Quartz;

namespace IRSI.PayrollDataGen
{
  public partial class PayrollSendJob : IJob
  {
    public IFtpSendPayroll FtpSendPayroll { get; set; }
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void Execute(IJobExecutionContext context)
    {
      _logger.Debug($"PayrollSendJob running at {context.FireTimeUtc?.ToLocalTime()}");

      FtpSendPayroll.RunJob();

      _logger.Debug($"Next PayrollSendJob will run at {context.NextFireTimeUtc?.ToLocalTime()}");
    }
  }
}
