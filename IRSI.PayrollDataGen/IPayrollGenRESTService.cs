using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace IRSI.PayrollDataGen
{
  [ServiceContract]
  public interface IPayrollGenRESTService
  {
    [OperationContract]
    [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "payrollGen?startDate={startDate}&endDate={endDate}")]
    string PayrollGenPost(string startDate, string endDate);
  }
}
