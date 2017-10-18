using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using IRSI.PayrollDataGen.Properties;
using NLog;

namespace IRSI.PayrollDataGen.Ftp
{
  public class FtpSendPayroll : IFtpSendPayroll
  {
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void SendFile(Uri uri, string username, string password, byte[] bytes)
    {
      _logger.Debug($"Uploading file {uri}");
      var request = (FtpWebRequest)WebRequest.Create(uri);
      request.Method = WebRequestMethods.Ftp.UploadFile;
      request.Credentials = new NetworkCredential(username, password);
      request.UsePassive = true;
      request.KeepAlive = false;
      request.ContentLength = bytes.Length;

      var requestStream = request.GetRequestStream();
      requestStream.Write(bytes, 0, bytes.Length);
      requestStream.Flush();
      requestStream.Close();

      var response = (FtpWebResponse)request.GetResponse();
      _logger.Debug($"Upload done: {response.StatusCode}");
      if (response != null) response.Close();
    }
  }
}
