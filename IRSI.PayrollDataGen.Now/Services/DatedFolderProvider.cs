using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IRSI.PayrollDataGen.Now.Services
{
  public class DatedFolderProvider : IDatedFolderProvider
  {
    public List<string> GetDatedFolders()
    {
      var iberdir = Environment.GetEnvironmentVariable("IBERDIR");
      return Directory.GetDirectories(iberdir, "20??????").Select(x => Path.GetFileName(x)).OrderByDescending(x => x)
          .Take(30).ToList();
    }

    public string GetDatedFolderFullName(string datePortion)
    {
      var iberdir = Environment.GetEnvironmentVariable("IBERDIR");
      return Path.Combine(iberdir, datePortion);
    }

  }
}
