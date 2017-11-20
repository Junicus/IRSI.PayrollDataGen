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

    public void SendFile(Uri uri, string username, string password, byte[] bytes, bool usePassive)
    {
      _logger.Debug($"Uploading file {uri}");
      try
      {
        var request = (FtpWebRequest)WebRequest.Create(uri);
        request.Method = WebRequestMethods.Ftp.UploadFile;
        request.Credentials = new NetworkCredential(username, password);
        request.UsePassive = usePassive;
        request.KeepAlive = false;
        request.ContentLength = bytes.Length;

        var requestStream = request.GetRequestStream();
        requestStream.Write(bytes, 0, bytes.Length);
        requestStream.Flush();
        requestStream.Close();

        var response = (FtpWebResponse)request.GetResponse();
        _logger.Debug($"Upload done: {response.StatusCode}");
        if (response != null) response.Close();
      } catch (WebException wex)
      {
        _logger.Error(wex, $"Error sending file to uri: {uri} with message: {wex.Message}");
      } catch (Exception ex)
      {
        _logger.Error(ex, $"Error uploading file with message: {ex.Message}");
      }
    }
  }
}
