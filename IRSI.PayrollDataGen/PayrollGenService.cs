using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Atlas;
using Autofac;
using Autofac.Integration.Wcf;
using IRSI.PayrollDataGen.Payroll;
using IRSI.PayrollDataGen.Properties;
using NLog;
using Quartz;

namespace IRSI.PayrollDataGen
{
  public class PayrollGenService : IAmAHostedProcess
  {
    private int m_GenIntervalInMinutes = Settings.Default.GenIntervalInMinutes;
    private int m_SendIntervalInMinutes = Settings.Default.SendIntervalInMinutes;
    private int m_StartHour = Settings.Default.StartHour;
    private int m_StartMinute = Settings.Default.StartMinute;

    public IScheduler Scheduler { get; set; }
    public IJobListener AutofacJobListener { get; set; }
    public ServiceHost Host { get; set; }

    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void Start()
    {
      _logger.Debug($"Starting Service");

      var genjob = JobBuilder.Create<PayrollGenJob>()
        .WithIdentity("Job1")
        .Build();

      var sendjob = JobBuilder.Create<PayrollSendJob>()
        .WithIdentity("Job2")
        .Build();

      var gentrigger = TriggerBuilder.Create().ForJob(genjob)
        .WithIdentity("Trigger1")
        .WithDailyTimeIntervalSchedule(x =>
          x.StartingDailyAt(new TimeOfDay(m_StartHour, m_StartMinute))
            .WithIntervalInMinutes(m_GenIntervalInMinutes)
        )
        .Build();

      var sendtrigger = TriggerBuilder.Create().ForJob(sendjob)
        .WithIdentity("Trigger2")
        .WithDailyTimeIntervalSchedule(x =>
          x.StartingDailyAt(new TimeOfDay(m_StartHour, m_StartMinute))
            .WithIntervalInMinutes(m_SendIntervalInMinutes)
        )
        .Build();

      Scheduler.ScheduleJob(genjob, gentrigger);
      Scheduler.ScheduleJob(sendjob, sendtrigger);

      _logger.Debug($"GenJob next run at {gentrigger.GetNextFireTimeUtc()?.ToLocalTime()}");
      _logger.Debug($"SendJob next run at {sendtrigger.GetNextFireTimeUtc()?.ToLocalTime()}");

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
