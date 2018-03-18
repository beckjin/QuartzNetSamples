using Quartz;
using Quartz.Impl;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            RunScheduler().GetAwaiter().GetResult();

            Console.ReadKey();
        }

        private static async Task RunScheduler()
        {
            // 创建作业调度器
            ISchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            // 开始运行调度器
            await scheduler.Start();

            // Job的附属信息
            var jobDataMap = new JobDataMap();
            jobDataMap.Add("name", "beck");
            jobDataMap.Add("times", 1);

            // 创建一个作业
            IJobDetail job = JobBuilder.Create<HelloJob>()
                .WithIdentity("job1", "jobGroup1")
                .UsingJobData(jobDataMap)
                .StoreDurably()
                .Build();

            // Trigger的附属信息
            var triggerDataMap = new JobDataMap();
            triggerDataMap.Add("age", 28);

            // 创建一个触发器
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "triggerGroup1")
                .UsingJobData(triggerDataMap)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(2)
                    .WithRepeatCount(10))
                .Build();

            // 加入作业调度器中
            await scheduler.ScheduleJob(job, trigger);

            await Task.Delay(TimeSpan.FromMinutes(2));

            // 关闭scheduler
            await scheduler.Shutdown(true);
        }
    }

    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class HelloJob : IJob
    {
        // 映射JobDetail.JobDataMap的name
        public string Name { get; set; }
    
        // 映射JobDetail.JobDataMap的times
        public int Times { get; set; }

        // 映射Trigger.JobDataMap的age
        public string Age { get; set; }

        /// <summary>
        /// 作业调度定时执行的方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            //var name = context.JobDetail.JobDataMap["name"].ToString();
            //var age = context.Trigger.JobDataMap["age"].ToString();
            //var name = context.MergedJobDataMap["name"].ToString();

            var fireTime = context.ScheduledFireTimeUtc?.ToLocalTime();
            var nextFireTime = context.NextFireTimeUtc?.ToLocalTime();

            context.JobDetail.JobDataMap["times"] = Times + 1;

            await Console.Out.WriteLineAsync($"execute {Times} times");

            // 测试并发
            Thread.Sleep(2000);
        }
    }
}
