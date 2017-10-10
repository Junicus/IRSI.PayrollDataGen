using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRSI.PayrollDataGen
{
  public class PayrollGenRESTService : IPayrollGenRESTService
  {
    public string PayrollGenPost(string startDate, string endDate)
    {
      Console.WriteLine($"{startDate} - {endDate}");
      return $"{startDate} - {endDate}";
    }
  }
}
