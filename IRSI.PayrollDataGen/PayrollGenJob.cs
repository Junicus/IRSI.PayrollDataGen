using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IRSI.PayrollDataGen.Payroll;
using Quartz;

namespace IRSI.PayrollDataGen
{
  public class PayrollGenJob : IJob
  {
    public IPayrollReader PayrollReader { get; set; }
    public IPayrollConverter PayrollConverter { get; set; }

    public void Execute(IJobExecutionContext context)
    {
      var iberdir = Environment.GetEnvironmentVariable("IBERDIR");
      var datedFolders = Directory.GetDirectories(iberdir, "20??????");

      foreach (var datedFolder in datedFolders)
      {
        var payrollData = PayrollReader.ReadPayroll(datedFolder);
        if (payrollData != null)
        {
          var payrollResult = PayrollConverter.ConvertPayroll(payrollData);
          foreach (var emp in payrollResult)
          {
            Console.WriteLine(emp.Trasactions.Count);
          }
        } else
        {
          Console.WriteLine("Error Reading Data");
        }
      }
    }
  }
}
