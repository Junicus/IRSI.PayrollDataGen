using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Atlas;
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

      Host = new ServiceHost(typeof(PayrollGenRESTService));
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
  }
}
