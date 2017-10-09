using System;
using System.Text;
using System.Xml.Serialization;

namespace IRSI.PayrollDataGen.Payroll.Model
{
  [Serializable]
  public class Store
  {
    public Store()
    {
      Employees = new Employees();
    }

    [XmlIgnore]
    public bool Checked { get; set; }

    [XmlAttribute]
    public int ID { get; set; }

    [XmlAttribute]
    public string StoreName { get; set; }

    [XmlElement]
    public Employees Employees { get; set; }

    [XmlIgnore]
    public decimal TotalHours {
      get => Employees.TotalHours;
    }

    [XmlIgnore]
    public decimal TotalPayCode {
      get => Employees.TotalPayCode;
    }
  }
}
