using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;


namespace IRSI.PayrollDataGen.Payroll.Model
{
  [Serializable()]
  public class Employee
  {
    private Transactions _transactions = new Transactions();

    [XmlAttribute()]
    public int ID { get; set; }

    [XmlIgnore()]
    public string BadgeNumber { get; set; }

    [XmlAttribute()]
    public string SocialSecurity { get; set; }

    [XmlAttribute()]
    public string FirstName { get; set; }

    [XmlAttribute()]
    public string LastName { get; set; }

    [XmlElement()]
    public Transactions Trasactions {
      get { return _transactions; }
      set { _transactions = value; }
    }

    [XmlIgnore()]
    public decimal TotalHours {
      get {
        if (_transactions.Count != 0)
        {
          var queryShitHours = _transactions.Where(t => t.Type == TransactionType.ClockInOut && t.PeriodType == PeriodType.Shift);
          var queryBreakHours = _transactions.Where(t => t.Type == TransactionType.ClockInOut && t.PeriodType == PeriodType.Break);
          return Math.Abs(queryShitHours.Sum(t => t.Hours)) - Math.Abs(queryBreakHours.Sum(t => t.Hours));
        } else
        {
          return 0m;
        }
      }
    }

    public decimal TotalPayCode {
      get {
        if (_transactions.Count != 0)
        {
          var queryPayCodes = _transactions.Where(t => t.Type == TransactionType.PayCode);
          return Math.Abs(queryPayCodes.Sum(t => t.Tips));
        } else
        {
          return 0m;
        }
      }
    }
  }
}