using CrystalQuartz.Owin;
using Microsoft.Owin;
using Owin;
using Quartz;
using Quartz.Impl;

[assembly: OwinStartup(typeof(Owin_WebSimple.Startup))]
namespace Owin_WebSimple
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            IJobDetail job = JobBuilder.Create<HelloJob>()
                .WithIdentity("job1", "group1")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);

            scheduler.Start();

            app.UseCrystalQuartz(scheduler);
        }
    }

    public class HelloJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
        }
    }
}