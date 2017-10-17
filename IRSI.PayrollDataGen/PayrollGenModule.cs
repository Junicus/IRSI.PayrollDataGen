using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas;
using Autofac;
using IRSI.PayrollDataGen.Ftp;
using IRSI.PayrollDataGen.Payroll;
using Quartz;
using Quartz.Impl;

namespace IRSI.PayrollDataGen
{
  public class PayrollGenModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      LoadQuartz(builder);
      LoadServices(builder);
    }

    private static void LoadQuartz(ContainerBuilder builder)
    {
      builder.Register(c => new StdSchedulerFactory().GetScheduler()).As<IScheduler>().InstancePerLifetimeScope();
      builder.Register(c => new AutofacJobListener(ContainerProvider.Instance)).As<IJobListener>();
    }

    private static void LoadServices(ContainerBuilder builder)
    {
      builder.RegisterType<PayrollGenService>().As<IAmAHostedProcess>().PropertiesAutowired();

      builder.RegisterType<PayrollReader>().As<IPayrollReader>().InstancePerLifetimeScope();
      builder.RegisterType<PayrollConverter>().As<IPayrollConverter>().InstancePerLifetimeScope();
      builder.RegisterType<PayrollXmlWriter>().As<IPayrollWriter>().InstancePerLifetimeScope();

      builder.RegisterType<FtpSendPayroll>().As<IFtpSendPayroll>().InstancePerLifetimeScope();
    }
  }
}
