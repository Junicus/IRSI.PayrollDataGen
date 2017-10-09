using System;
using System.Xml.Serialization;

namespace IRSI.PayrollDataGen.Payroll.Model
{
  [Serializable]
  public class Transaction
  {
    public Transaction()
    {
      ClockIn = DateTime.MinValue;
      ClockOut = DateTime.MinValue;
      PeriodType = PeriodType.Unknown;
    }

    [XmlAttribute]
    public PeriodType PeriodType { get; set; }

    [XmlAttribute]
    public int EmpID { get; set; }

    [XmlAttribute]
    public string SSN { get; set; }

    [XmlAttribute]
    public TransactionType Type { get; set; }

    [XmlElement]
    public int JobCode { get; set; }

    [XmlElement]
    public DateTime ClockIn { get; set; }

    [XmlElement]
    public DateTime ClockOut { get; set; }

    [XmlElement]
    public string PayCode { get; set; }

    [XmlElement]
    public decimal Tips { get; set; }

    [XmlElement]
    public decimal Hours { get; set; }
  }
}
