using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using IRSI.PayrollDataGen.Payroll.Model;
using IRSI.PayrollDataGen.Properties;
using NLog;

namespace IRSI.PayrollDataGen.Payroll
{
  public class PayrollXmlWriter : IPayrollWriter
  {
    private readonly ILogger _logger;

    public PayrollXmlWriter()
    {
      _logger = LogManager.GetCurrentClassLogger();
    }

    public void WriteFile(List<Employee> employees, string filename)
    {
      _logger.Info($"Write Payroll called");
      using (var streamWriter = new StreamWriter(filename))
      {
        WriteStream(employees, streamWriter);
      }
      _logger.Info($"Write Payroll finished");
    }

    public void WriteStream(List<Employee> employees, StreamWriter streamWriter)
    {
      _logger.Debug($"Serializing employees with transactions");
      var employeesWithTransactions = employees.Where(t => t.Transactions.Any());
      var employeesCollection = new Employees();
      employeesCollection.AddRange(employeesWithTransactions);
      var store = new Store {
        ID = Settings.Default.StoreId,
        StoreName = Settings.Default.StoreName,
        Employees = employeesCollection
      };
      var serializer = new XmlSerializer(typeof(Store));
      try
      {
        serializer.Serialize(streamWriter, store);
        streamWriter.Flush();
      } catch (Exception e)
      {
        _logger.Error(e, "Error serializing payroll");
      }
    }
  }
}
