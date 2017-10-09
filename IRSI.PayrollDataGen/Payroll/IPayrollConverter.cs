using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IRSI.PayrollDataGen.Payroll.Model;

namespace IRSI.PayrollDataGen.Payroll
{
  public interface IPayrollConverter
  {
    List<Employee> ConvertPayroll(AlohaData alohaData);
  }
}
