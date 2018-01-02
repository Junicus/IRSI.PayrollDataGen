using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using IRSI.PayrollDataGen.Ftp.Model;
using Newtonsoft.Json.Linq;
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

    public FtpSetting ParseFtpSettingsLine(string setting)
    {
      var parts = setting.Split(' ');
      var pathParts = parts[1].Split(',');

      var path = String.Join("/", pathParts);

      var ftpSetting = new FtpSetting {
        Url = parts[0],
        Path = path,
        Username = parts[2],
        Password = parts[3],
        UsePassive = bool.Parse(parts[4])
      };

      return ftpSetting;
    }

    public void RunJob()
    {
      try
      {
        var currentFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        var ftpSettings = new List<FtpSetting>();
        var json = File.ReadAllText(Path.Combine(currentFolder, "ftpurl.json"));
        var o = JObject.Parse(json);
        var urls = o["ftpSettings"].Select(u => (string)u).ToList();
        var ftpOutputDirectory = Path.Combine(currentFolder, "ftpOutput");

        foreach (var url in urls)
        {
          ftpSettings.Add(ParseFtpSettingsLine(url));
        }

        foreach (var ftpSetting in ftpSettings)
        {
          if (!Directory.Exists(ftpOutputDirectory))
          {
            Directory.CreateDirectory(ftpOutputDirectory);
          }
          foreach (var file in Directory.GetFiles(ftpOutputDirectory))
          {
            var uri = new Uri($"ftp://{ftpSetting.Url}/{ftpSetting.Path}/{Path.GetFileName(file)}");
            SendFile(uri, ftpSetting.Username, ftpSetting.Password, File.ReadAllBytes(file), ftpSetting.UsePassive);
            File.Delete(file);
          }
        }
      } catch (Exception ex)
      {
        _logger.Error(ex.Message);
      }
    }
  }
}
