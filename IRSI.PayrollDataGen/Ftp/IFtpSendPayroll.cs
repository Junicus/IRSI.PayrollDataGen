using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRSI.PayrollDataGen.Ftp
{
  public interface IFtpSendPayroll
  {
    void SendFiles(string folderName, string url, string username, string password);
  }
}
