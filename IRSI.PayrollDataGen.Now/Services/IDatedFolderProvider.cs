using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRSI.PayrollDataGen.Now.Services
{
  public interface IDatedFolderProvider
  {
    List<string> GetDatedFolders();
    string GetDatedFolderFullName(string datePortion);
  }
}
