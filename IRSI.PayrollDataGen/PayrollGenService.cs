using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Atlas;
using Autofac;
using Autofac.Integration.Wcf;
using IRSI.PayrollDataGen.Payroll;
using Quartz;

namespace IRSI.PayrollDataGen
{
  public class PayrollGenService : IAmAHostedProcess
  {
    const int IntervalInMinutes = 1;

    public IScheduler Scheduler { get; set; }
    public IJobListener AutofacJobListener { get; set; }
    public ServiceHost Host { get; set; }

    public void Start()
    {
      var job = JobBuilder.Create<PayrollGenJob>()
      .WithIdentity("Job1")
      .Build();

      var trigger = TriggerBuilder.Create()
      .WithIdentity("Tigger1")
      .StartNow()
      .WithCalendarIntervalSchedule(x => x.WithIntervalInMinutes(IntervalInMinutes))
      .Build();

      Scheduler.ScheduleJob(job, trigger);
      Scheduler.ListenerManager.AddJobListener(AutofacJobListener);
      Scheduler.Start();

      var container = SetupPayrollGenRESTServiceContainer();
      Host = new ServiceHost(typeof(PayrollGenRESTService));
      Host.AddDependencyInjectionBehavior<IPayrollGenRESTService>(container);
      Host.Open();
    }

    public void Stop()
    {
      Scheduler.Shutdown();
      Host.Close();
    }

    public void Resume()
    {
      Scheduler.ResumeAll();
    }

    public void Pause()
    {
      Scheduler.PauseAll();
    }

    private IContainer SetupPayrollGenRESTServiceContainer()
    {
      var builder = new ContainerBuilder();

      builder.RegisterType<PayrollReader>().As<IPayrollReader>();
      builder.RegisterType<PayrollConverter>().As<IPayrollConverter>();
      builder.RegisterType<PayrollXmlWriter>().As<IPayrollWriter>();

      builder.RegisterType<PayrollGenRESTService>().As<IPayrollGenRESTService>();

      return builder.Build();
    }
  }
}
