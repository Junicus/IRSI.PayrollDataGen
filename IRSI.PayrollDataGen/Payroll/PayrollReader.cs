using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IRSI.PayrollDataGen.Payroll.AlohaDataTableAdapters;

namespace IRSI.PayrollDataGen.Payroll
{
  public class PayrollReader : IPayrollReader
  {
    public AlohaData ReadPayroll(string datedFolder)
    {
      var connectionString = GetConnectionString(datedFolder);
      try
      {
        var data = new AlohaData();

        var empAdapter = new empTableAdapter();
        empAdapter.Connection.ConnectionString = connectionString;
        empAdapter.Fill(data.emp);

        var adjTimeAdapter = new adjtimeTableAdapter();
        adjTimeAdapter.Connection.ConnectionString = connectionString;
        adjTimeAdapter.Fill(data.adjtime);

        var gndBreakAdapter = new gndbreakTableAdapter();
        gndBreakAdapter.Connection.ConnectionString = connectionString;
        gndBreakAdapter.Fill(data.gndbreak);

        return data;
      } catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }

      return null;
    }

    private string GetConnectionString(string datedFolder)
    {
      return $"Provider=VFPOLEDB.1;Data Source={datedFolder}";
    }
  }
}
