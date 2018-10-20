using Quartz;
using Quartz.Impl;
using System;
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

        public static async Task RunScheduler()
        {
            // 创建作业调度器
            ISchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            await scheduler.Start();

            var jobDataMap = new JobDataMap();
            jobDataMap.Add("times", "1");

            // 创建一个作业
            IJobDetail job = JobBuilder.Create<HelloJob>()
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

            var jobExist = await scheduler.CheckExists(job.Key);
            if (!jobExist)
            {
                await scheduler.ScheduleJob(job, trigger);
            }
        }
    }

    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class HelloJob : IJob
    {
        /// <summary>
        /// 作业调度定时执行的方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            var times = Convert.ToInt32(context.JobDetail.JobDataMap["times"]);
            context.JobDetail.JobDataMap["times"] = (times + 1).ToString();

            await Console.Out.WriteLineAsync($"execute {times} times");
        }
    }
}
