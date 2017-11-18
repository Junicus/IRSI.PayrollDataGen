using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IRSI.PayrollDataGen.Ftp;
using IRSI.PayrollDataGen.Properties;
using Newtonsoft.Json.Linq;
using NLog;
using Quartz;

namespace IRSI.PayrollDataGen
{
  public class PayrollSendJob : IJob
  {
    public IFtpSendPayroll FtpSendPayroll { get; set; }
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void Execute(IJobExecutionContext context)
    {
      var currentFolder = Directory.GetCurrentDirectory();
      var ftpSettings = new List<FtpSetting>();
      var json = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "ftpurl.json"));
      var o = JObject.Parse(json);
      var urls = o["ftpSettings"].Select(u => (string)u).ToList();
      var ftpOutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "ftpOutput");

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
          //FtpSendPayroll.SendFile(uri, ftpSetting.Username, ftpSetting.Password, File.ReadAllBytes(file));
        }
      }

      return;
    }

    private FtpSetting ParseFtpSettingsLine(string setting)
    {
      var parts = setting.Split(' ');
      var pathParts = parts[1].Split(',');

      var path = String.Join("/", pathParts);

      var ftpSetting = new FtpSetting {
        Url = parts[0],
        Path = path,
        Username = parts[2],
        Password = parts[3]
      };

      return ftpSetting;
    }

    private class FtpSetting
    {
      public string Url { get; set; }
      public string Username { get; set; }
      public string Password { get; set; }
      public string Path { get; set; }
    }
  }
}
