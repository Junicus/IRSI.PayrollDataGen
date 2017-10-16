using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Web;
using System.Text;
using IRSI.PayrollDataGen.Payroll;
using IRSI.PayrollDataGen.Payroll.Model;

namespace IRSI.PayrollDataGen
{
  public class PayrollGenRESTService : IPayrollGenRESTService
  {
    private readonly IPayrollReader _payrollReader;
    private readonly IPayrollConverter _payrollConverter;
    private readonly IPayrollWriter _payrollWriter;

    public PayrollGenRESTService(IPayrollReader payrollReader, IPayrollConverter payrollConverter, IPayrollWriter payrollWriter)
    {
      _payrollReader = payrollReader;
      _payrollConverter = payrollConverter;
      _payrollWriter = payrollWriter;
    }

    public string PayrollGenPost(string startDate, string endDate)
    {
      var datedStart = DateTime.ParseExact(startDate, "yyyyMMdd", CultureInfo.InvariantCulture);
      var datedEnd = DateTime.ParseExact(endDate, "yyyyMMdd", CultureInfo.InvariantCulture);
      var iberdir = Environment.GetEnvironmentVariable("IBERDIR");

      var datedFolders = new List<string>();
      for (var currentDate = datedStart; currentDate <= datedEnd; currentDate = currentDate.AddDays(1))
      {
        datedFolders.Add(Path.Combine(iberdir, currentDate.ToString("yyyyMMdd")));
      }

      var alohaDatasets = new List<AlohaData>();
      foreach (var datedFolder in datedFolders)
      {
        alohaDatasets.Add(_payrollReader.ReadPayroll(datedFolder));
      }

      if (alohaDatasets.Any(t => t == null))
      {
        OutgoingWebResponseContext badresult = WebOperationContext.Current.OutgoingResponse;
        badresult.StatusCode = HttpStatusCode.BadRequest;
        badresult.StatusDescription = "Invalid date range, one or more dated folders not found in range";
        return null;
      }

      var employeeLists = new List<List<Employee>>();
      foreach (var dataSet in alohaDatasets)
      {
        employeeLists.Add(_payrollConverter.ConvertPayroll(dataSet));
      }

      var flatEmployees = FlattenEmployeeList(employeeLists);
      var result = string.Empty;

      using (var memoryStream = new MemoryStream())
      {
        using (var streamWriter = new StreamWriter(memoryStream))
        {
          _payrollWriter.WriteStream(flatEmployees, streamWriter);
          using (var sr = new StreamReader(memoryStream))
          {
            memoryStream.Position = 0;
            result = sr.ReadToEnd();
          }
        }
      }

      return result;
    }

    private List<Employee> FlattenEmployeeList(List<List<Employee>> employeeLists)
    {
      var result = new List<Employee>();

      foreach (var employeeList in employeeLists)
      {
        foreach (var employee in employeeList)
        {
          if (result.Any(t => t.ID == employee.ID))
          {
            result.SingleOrDefault(t => t.ID == employee.ID).Trasactions.AddRange(employee.Trasactions);
          } else
          {
            result.Add(employee);
          }
        }
      }

      return result;
    }
  }
}
