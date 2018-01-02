namespace IRSI.PayrollDataGen.Ftp.Model
{
  public class FtpSetting
  {
    public string Url { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Path { get; set; }
    public bool UsePassive { get; set; }
  }
}
