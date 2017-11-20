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
    const int IntervalInHours = 2;
    const int IntervalInDays = 1;

    public IScheduler Scheduler { get; set; }
    public IJobListener AutofacJobListener { get; set; }
    public ServiceHost Host { get; set; }

    public void Start()
    {
      var genjob = JobBuilder.Create<PayrollGenJob>()
        .WithIdentity("Job1")
        .Build();

      var sendjob = JobBuilder.Create<PayrollSendJob>()
        .WithIdentity("Job2")
        .Build();

      var gentrigger = TriggerBuilder.Create().ForJob(genjob)
        .WithIdentity("Trigger1")
        .StartAt(DateBuilder.DateOf(5, 0, 0))
        .WithSimpleSchedule(x => x.WithIntervalInMinutes(10))
        .EndAt(DateBuilder.DateOf(6, 0, 0))
        .Build();

      var sendtrigger = TriggerBuilder.Create().ForJob(sendjob)
        .WithIdentity("Trigger2")
        .StartAt(DateBuilder.DateOf(5, 0, 0))
        .WithSimpleSchedule(x => x.WithIntervalInMinutes(15))
        .EndAt(DateBuilder.DateOf(6, 0, 0))
        .Build();

      Scheduler.ScheduleJob(genjob, gentrigger);
      Scheduler.ScheduleJob(sendjob, sendtrigger);
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
