using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IRSI.PayrollDataGen.Payroll.Model;

namespace IRSI.PayrollDataGen.Payroll
{
  public interface IPayrollWriter
  {
    void WriteFile(List<Employee> employees, string filename, int storeId, string storeName);
    void WriteStream(List<Employee> employees, StreamWriter streamWriter, int storeId, string storeName);
  }
}
