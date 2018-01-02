using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IRSI.PayrollDataGen.Ftp.Model;

namespace IRSI.PayrollDataGen.Ftp
{
  public interface IFtpSendPayroll
  {
    void SendFile(Uri uri, string username, string password, byte[] bytes, bool usePassive);
    FtpSetting ParseFtpSettingsLine(string setting);
    void RunJob();
  }
}
