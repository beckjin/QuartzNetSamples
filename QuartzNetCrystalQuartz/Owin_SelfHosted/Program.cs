using CrystalQuartz.Owin;
using Microsoft.Owin.Hosting;
using Owin;
using Quartz;
using Quartz.Impl;
using System;

namespace Owin_SelfHosted
{
    class Program
    {
        static void Main()
        {
            IScheduler scheduler = SetupScheduler();
            Action<IAppBuilder> startup = app =>
            {
                app.UseCrystalQuartz(scheduler);
            };

            using (WebApp.Start("http://localhost:9000/", startup))
            {
                Console.WriteLine("Check http://localhost:9000/quartz to see jobs information");
                scheduler.Start();
                Console.WriteLine("Press any key to close");
                Console.ReadLine();
            }
            scheduler.Shutdown(waitForJobsToComplete: true);
        }

        private static IScheduler SetupScheduler()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            IJobDetail job = JobBuilder.Create<PrintMessageJob>()
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

            return scheduler;
        }
    }

    public class PrintMessageJob : IJob
    {
        void IJob.Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Hello QuartzNet");
        }
    }
}
