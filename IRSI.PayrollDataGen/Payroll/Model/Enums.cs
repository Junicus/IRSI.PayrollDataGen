using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRSI.PayrollDataGen.Payroll.Model
{
  public enum TransactionType
  {
    ClockInOut,
    PayCode
  }

  public enum PeriodType
  {
    Unknown,
    Shift,
    Break
  }

  public enum TipsCalculation
  {
    Auto,
    CCTips,
    DecTips
  }
}
