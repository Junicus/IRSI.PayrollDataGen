using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRSI.PayrollDataGen.Payroll
{
  public interface IPayrollConverter
  {
	List<PayrollEntry> ConvertPayroll(AlohaData alohaData);
  }
}
