using Quartz;
using Quartz.Impl;

namespace App_NetFramework
{
    public class TestSchedule
    {
        readonly IScheduler scheduler = null;

        public TestSchedule()
        {
            // 创建作业调度器
            scheduler = new StdSchedulerFactory().GetScheduler().Result;
            var jobDataMap = new JobDataMap();
            jobDataMap.Add("times", "1");

            // 创建一个作业
            IJobDetail job = JobBuilder.Create<TestJob>()
                .WithIdentity("job1", "jobGroup1")
                .UsingJobData(jobDataMap)
                .Build();

            // 创建一个触发器
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "triggerGroup1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(1)
                    .WithRepeatCount(10))
                .Build();

            scheduler.ScheduleJob(job, trigger).Wait();
        }

        public void Start()
        {
            scheduler.Start().Wait();
        }

        public void Stop()
        {
            scheduler.Shutdown().Wait();
        }
    }
}
