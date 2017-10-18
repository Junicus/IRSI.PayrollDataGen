using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRSI.PayrollDataGen.Ftp
{
  public interface IFtpSendPayroll
  {
    void SendFile(Uri uri, string username, string password, byte[] bytes);
  }
}
