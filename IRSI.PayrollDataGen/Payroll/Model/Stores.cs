using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace IRSI.PayrollDataGen.Payroll.Model
{
  [Serializable]
  [XmlRoot("Stores", Namespace = "http://www.irsipr.com", IsNullable = false)]
  public class Stores : List<Store>
  {
    [XmlIgnore]
    public List<Store> Checked {
      get {
        List<Store> retval = new List<Store>();
        var ch_stores = this.Where(c => c.Checked);
        retval.AddRange(ch_stores);
        return retval;
      }
    }

    [XmlIgnore]
    public decimal TotalHours {
      get => this.Sum(t => t.TotalHours);
    }

    [XmlIgnore]
    public decimal TotalPayCodes {
      get => this.Sum(t => t.TotalPayCode);
    }
  }
}
