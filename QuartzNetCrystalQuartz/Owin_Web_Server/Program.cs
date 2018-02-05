using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;

namespace Owin_Web_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var scheduler = GetScheduler();
            scheduler.Start();
            Console.WriteLine("Press any key to close");
            Console.ReadLine();
            scheduler.Shutdown();
        }

        private static IScheduler GetScheduler()
        {
            var properties = new NameValueCollection();
            properties["quartz.scheduler.instanceName"] = "RemoteServerSchedulerClient";

            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "5";
            properties["quartz.threadPool.threadPriority"] = "Normal";

            properties["quartz.scheduler.exporter.type"] = "Quartz.Simpl.RemotingSchedulerExporter, Quartz";
            properties["quartz.scheduler.exporter.port"] = "555";
            properties["quartz.scheduler.exporter.bindName"] = "QuartzScheduler";
            properties["quartz.scheduler.exporter.channelType"] = "tcp";

            var schedulerFactory = new StdSchedulerFactory(properties);
            var scheduler = schedulerFactory.GetScheduler();

            var map = new JobDataMap();
            map.Put("msg", "Some message!");

            var job = JobBuilder.Create<PrintMessageJob>()
               .WithIdentity("localJob", "default")
               .UsingJobData(map)
               .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("remotelyAddedTrigger", "default")
                .ForJob(job)
                .StartNow()
                .WithCronSchedule("/5 * * ? * *")
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
