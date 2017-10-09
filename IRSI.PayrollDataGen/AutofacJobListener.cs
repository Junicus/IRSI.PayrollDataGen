using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas;
using Quartz;

namespace IRSI.PayrollDataGen
{
  public class AutofacJobListener : IJobListener
  {
	private readonly IContainerProvider _containerProvider;
	private IUnitOfWorkContainer _container;

	public AutofacJobListener(IContainerProvider containerProvider)
	{
	  _containerProvider = containerProvider;
	}

	public string Name => "AutofacInjectionJobListener";

	public void JobToBeExecuted(IJobExecutionContext context)
	{
	  _container = _containerProvider.CreateUnitOfWork();
	  _container.InjectUnsetProperties(context.JobInstance);
	}

	public void JobExecutionVetoed(IJobExecutionContext context)
	{
	}

	public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
	{
	  _container.Dispose();
	}
  }
}
