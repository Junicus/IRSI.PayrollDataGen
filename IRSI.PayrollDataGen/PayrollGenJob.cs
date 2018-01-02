using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using IRSI.PayrollDataGen.Payroll;
using IRSI.PayrollDataGen.Properties;
using NLog;
using Quartz;

namespace IRSI.PayrollDataGen
{
  public class PayrollGenJob : IJob
  {
    public IPayrollReader PayrollReader { get; set; }
    public IPayrollConverter PayrollConverter { get; set; }
    public IPayrollWriter PayrollWriter { get; set; }
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void Execute(IJobExecutionContext context)
    {
      var iberdir = Environment.GetEnvironmentVariable("IBERDIR");
      var datedFolders = Directory.GetDirectories(iberdir, "20??????");
      var currentFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
      var ftpOutputFolder = Path.Combine(currentFolder, "ftpOutput");
      var portalOutputFolder = Path.Combine(currentFolder, "portalOutput");

      _logger.Debug($"PayrollGenJob running at {context.FireTimeUtc?.ToLocalTime()}");

      foreach (var datedFolder in datedFolders)
      {
        //_logger.Info($"Checking: {datedFolder}");
        var markerFilePath = Path.Combine(datedFolder, "IRSIPAYGEN");
        if (!File.Exists(markerFilePath))
        {
          _logger.Info($"Marker file for {datedFolder} not found, generating payroll");
          _logger.Debug($"Begin reading aloha data");
          var payrollData = PayrollReader.ReadPayroll(datedFolder);
          _logger.Debug($"Finished reading aloha data");
          if (payrollData != null)
          {
            _logger.Debug($"Begin converting payroll");
            var payrollResult = PayrollConverter.ConvertPayroll(payrollData, Settings.Default.TipCalculationStrategy);
            _logger.Debug($"Finish converting payroll");

            _logger.Info($"Checking output folders and creating if not found");
            if (!Directory.Exists(ftpOutputFolder)) Directory.CreateDirectory(ftpOutputFolder);
            if (!Directory.Exists(portalOutputFolder)) Directory.CreateDirectory(portalOutputFolder);

            _logger.Debug($"Begin writing files");
            _logger.Debug($"Writing {GetOutputFilename(datedFolder, ftpOutputFolder)}");
            PayrollWriter.WriteFile(payrollResult, GetOutputFilename(datedFolder, ftpOutputFolder), Settings.Default.StoreId, Settings.Default.StoreName);
            _logger.Debug($"Writing {GetOutputFilename(datedFolder, portalOutputFolder)}");
            PayrollWriter.WriteFile(payrollResult, GetOutputFilename(datedFolder, portalOutputFolder), Settings.Default.StoreId, Settings.Default.StoreName);
            _logger.Debug($"Finished writing files");
          }
          using (var filestream = File.Create(markerFilePath)) { };
        } else
        {
          //_logger.Info($"Marker file found, skipping");
        }
      }

      _logger.Debug($"Next PayrollGenJob will run at {context.NextFireTimeUtc?.ToLocalTime()}");
    }

    private string GetOutputFilename(string datedFolderName, string outputPath)
    {
      var storeId = Settings.Default.StoreId;
      var date = datedFolderName.Substring(datedFolderName.LastIndexOf('\\') + 1);
      var fileName = $"{storeId}-{date}.xml";
      return Path.Combine(outputPath, fileName);
    }
  }
}
