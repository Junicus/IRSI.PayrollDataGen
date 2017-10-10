using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IRSI.PayrollDataGen.Payroll;
using IRSI.PayrollDataGen.Properties;
using Quartz;

namespace IRSI.PayrollDataGen
{
  public class PayrollGenJob : IJob
  {
    public IPayrollReader PayrollReader { get; set; }
    public IPayrollConverter PayrollConverter { get; set; }
    public IPayrollWriter PayrollWriter { get; set; }

    public void Execute(IJobExecutionContext context)
    {
      var iberdir = Environment.GetEnvironmentVariable("IBERDIR");
      var datedFolders = Directory.GetDirectories(iberdir, "20??????");
      var currentFolder = Directory.GetCurrentDirectory();
      var ftpOutputFolder = Path.Combine(currentFolder, "ftpOutput");
      var portalOutputFolder = Path.Combine(currentFolder, "portalOutput");

      foreach (var datedFolder in datedFolders)
      {
        var payrollData = PayrollReader.ReadPayroll(datedFolder);
        if (payrollData != null)
        {
          var payrollResult = PayrollConverter.ConvertPayroll(payrollData);

          if (!Directory.Exists(ftpOutputFolder)) Directory.CreateDirectory(ftpOutputFolder);
          if (!Directory.Exists(portalOutputFolder)) Directory.CreateDirectory(portalOutputFolder);

          PayrollWriter.WriteFile(payrollResult, GetOutputFilename(datedFolder, ftpOutputFolder));
          PayrollWriter.WriteFile(payrollResult, GetOutputFilename(datedFolder, portalOutputFolder));

        } else
        {
          Console.WriteLine("Error Reading Data");
        }
      }
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
