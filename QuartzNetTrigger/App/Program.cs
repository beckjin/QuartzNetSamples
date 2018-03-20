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

        private static async Task RunScheduler()
        {
            // 创建作业调度器
            ISchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            // 开始运行调度器
            await scheduler.Start();

            // 创建一个作业
            IJobDetail job = JobBuilder.Create<HelloJob>()
                .WithIdentity("job1", "jobGroup1")
                .StoreDurably()
                .Build();

            // 设置trigger开始时间
            var startAt = DateTimeOffset.Now;

            // trigger的附属信息
            var triggerDataMap = new JobDataMap();
            triggerDataMap.Add("name", "beck");

            // 创建触发器
            ITrigger trigger1 = TriggerBuilder.Create()
                .WithIdentity("trigger1", "triggerGroup1")
                .StartAt(startAt)
                .WithCronSchedule("0/2 * * * * ?")
                .UsingJobData(triggerDataMap)
                .WithPriority(3)
                .Build();

            // 创建触发器
            ITrigger trigger2 = TriggerBuilder.Create()
                .WithIdentity("trigger2", "triggerGroup1")
                .StartAt(startAt)
                .EndAt(DateTimeOffset.Now.AddHours(1))
                .WithCronSchedule("0/2 * * * * ?")
                .ForJob(job)
                .UsingJobData(triggerDataMap)
                .WithPriority(7)
                .Build();

            // 加入作业调度器中
            await scheduler.ScheduleJob(job, trigger1);
            await scheduler.ScheduleJob(trigger2);

            await Task.Delay(TimeSpan.FromMinutes(2));

            // 关闭scheduler
            await scheduler.Shutdown(true);
        }
    }

    public class HelloJob : IJob
    {
        /// <summary>
        /// 作业调度定时执行的方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            //var name = context.Trigger.JobDataMap["name"].ToString();
            var keyName = context.Trigger.Key.Name;
            await Console.Out.WriteLineAsync($"triggerKeyName:{keyName}");
        }
    }
}
