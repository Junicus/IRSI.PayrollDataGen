using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IRSI.PayrollDataGen.Payroll;

namespace IRSI.PayrollDataGen
{
  public class PayrollGenRESTService : IPayrollGenRESTService
  {
    public IPayrollConverter PayrollConverter { get; set; }

    public string PayrollGenPost(string startDate, string endDate)
    {
      Console.WriteLine($"{PayrollConverter}");
      Console.WriteLine($"{startDate} - {endDate}");
      return $"{startDate} - {endDate}";
    }
  }
}
