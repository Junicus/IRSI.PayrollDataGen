using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace IRSI.PayrollDataGen
{
  [ServiceContract]
  public interface IPayrollGenRESTService
  {
    [WebInvoke(Method = "POST", UriTemplate = "payrollGen")]
    string PayrollGenPost(string startDate, string endDate);
  }
}
