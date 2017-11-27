using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateMarker
{
  class Program
  {
    static void Main(string[] args)
    {
      var iberdir = Environment.GetEnvironmentVariable("IBERDIR");
      var datedFolders = Directory.GetDirectories(iberdir, "20??????");
      foreach(var datedFolder in datedFolders)
      {
        var markerFilePath = Path.Combine(datedFolder, "IRSIPAYGEN");
        if(!File.Exists(markerFilePath))
        {
          using (var filestream = File.Create(markerFilePath)) { };
        }
      }
    }
  }
}
