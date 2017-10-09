using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRSI.PayrollDataGen.Payroll
{
  public interface IPayrollReader
  {
    AlohaData ReadPayroll(string datedFolder);
  }
}
