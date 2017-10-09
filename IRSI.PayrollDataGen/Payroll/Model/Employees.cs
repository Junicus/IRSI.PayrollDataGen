using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;


namespace IRSI.PayrollDataGen.Payroll.Model
{
  [Serializable]
  [XmlRoot("Employees", Namespace = "http://www.irsipr.com", IsNullable = false)]
  public class Employees : List<Employee>
  {
    [XmlIgnore]
    public decimal TotalHours {
      get {
        return this.Sum(t => t.TotalHours);
      }
    }

    [XmlIgnore]
    public decimal TotalPayCode {
      get {
        return this.Sum(t => t.TotalPayCode);
      }
    }
  }
}