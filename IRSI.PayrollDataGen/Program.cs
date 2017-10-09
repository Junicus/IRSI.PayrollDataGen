using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas;
using Autofac;

namespace IRSI.PayrollDataGen
{
  class Program
  {
	static void Main(string[] args)
	{
	  var configuration = Host.UseAppConfig<PayrollGenService>()
		.AllowMultipleInstances()
		.WithRegistrations(b => b.RegisterModule(new PayrollGenModule()))
		.WithArguments(args);
	  Host.Start(configuration);
	}
  }
}
