using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using IRSI.PayrollDataGen.Now.Properties;
using IRSI.PayrollDataGen.Now.Services;
using IRSI.PayrollDataGen.Payroll;
using NLog;

namespace IRSI.PayrollDataGen.Now
{
  public partial class frmGeneratePayroll : Form
  {
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    private readonly IPayrollReader _payrollReader;
    private readonly IPayrollConverter _payrollConverter;
    private readonly IPayrollWriter _payrollWriter;
    private readonly IDatedFolderProvider _datedFolderProvider;

    private readonly string _ftpOutputFolder;
    private readonly string _portalOutputFolder;

    public frmGeneratePayroll(IPayrollReader payrollReader, IPayrollConverter payrollConverter, IPayrollWriter payrollWriter, IDatedFolderProvider datedFolderProvider)
    {
      _payrollReader = payrollReader;
      _payrollConverter = payrollConverter;
      _payrollWriter = payrollWriter;
      _datedFolderProvider = datedFolderProvider;

      InitializeComponent();

      var currentFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
      _ftpOutputFolder = Path.Combine(currentFolder, "ftpOutput");
      _portalOutputFolder = Path.Combine(currentFolder, "portalOutput");

      _datedFolderProvider.GetDatedFolders().ForEach(x => this.lvDatedFolders.Items.Add(x));
    }

    private void cmdExit_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void lvDatedFolders_ItemChecked(object sender, ItemCheckedEventArgs e)
    {
      if (lvDatedFolders.CheckedItems.Count > 0)
      {
        cmdGenerate.Enabled = true;
      } else
      {
        cmdGenerate.Enabled = false;
      }
    }

    private void cmdGenerate_Click(object sender, EventArgs e)
    {
      var startTime = DateTime.Now;
      _logger.Debug($"Generate payroll started at {startTime.ToLocalTime()}");
      tbLog.Text += $"Generate payroll started at {startTime.ToLocalTime()}{Environment.NewLine}";

      foreach (var checkedItem in lvDatedFolders.CheckedItems)
      {
        _logger.Debug($"Checked Folder {((ListViewItem)checkedItem).Text}");
        tbLog.Text += $"Checked Folder {((ListViewItem)checkedItem).Text}{Environment.NewLine}";

        var folderName = _datedFolderProvider.GetDatedFolderFullName(((ListViewItem)checkedItem).Text);
        var markerFilePath = Path.Combine(folderName, "IRSIPAYGEN");

        _logger.Debug($"Begin reading aloha data");
        var payrollData = _payrollReader.ReadPayroll(folderName);
        _logger.Debug($"Finished reading aloha data");
        if (payrollData != null)
        {
          _logger.Debug($"Begin converting payroll");
          var payrollResult = _payrollConverter.ConvertPayroll(payrollData, Settings.Default.TipCalculationStrategy);
          _logger.Debug($"Finish converting payroll");

          _logger.Info($"Checking output folders and creating if not found");
          if (!Directory.Exists(_ftpOutputFolder)) Directory.CreateDirectory(_ftpOutputFolder);
          if (!Directory.Exists(_portalOutputFolder)) Directory.CreateDirectory(_portalOutputFolder);

          _logger.Debug($"Begin writing files");
          _logger.Debug($"Writing {GetOutputFilename(folderName, _ftpOutputFolder)}");
          _payrollWriter.WriteFile(payrollResult, GetOutputFilename(folderName, _ftpOutputFolder), Settings.Default.StoreId, Settings.Default.StoreName);
          _logger.Debug($"Writing {GetOutputFilename(folderName, _portalOutputFolder)}");
          _payrollWriter.WriteFile(payrollResult, GetOutputFilename(folderName, _portalOutputFolder), Settings.Default.StoreId, Settings.Default.StoreName);
          _logger.Debug($"Finished writing files");

          using (var filestream = File.Create(markerFilePath)) { }
        }
      }

      var endTime = DateTime.Now;
      _logger.Debug($"Generate payroll finished at {endTime.ToLocalTime()}");
      tbLog.Text += $"Generate payroll finished at {endTime.ToLocalTime()}{Environment.NewLine}";
      var elapsedTime = endTime - startTime;

      _logger.Debug($"Generate payroll took {elapsedTime.Milliseconds}ms");
      tbLog.Text += $"Generate payroll took {elapsedTime.Milliseconds}ms{Environment.NewLine}";
    }

    private string GetOutputFilename(string datedFolderName, string outputPath)
    {
      var storeId = Settings.Default.StoreId;
      var date = datedFolderName.Substring(datedFolderName.LastIndexOf('\\') + 1);
      var fileName = $"{storeId}-{date}.xml";
      return Path.Combine(outputPath, fileName);
    }
  }
}
