using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IRSI.PayrollDataGen.Payroll
{
  public class PayrollReader : IPayrollReader
  {
	public string ReadPayroll(string datedFolder)
	{
	  return GetConnectionString(datedFolder);
	}

	private string GetConnectionString(string datedFolder)
	{
	  return $"Provider=VFPOLEDB.1;Data Source={datedFolder}";
	}
  }
}
