using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Threading.Tasks;

namespace App2
{
    class Program
    {
        static void Main(string[] args)
        {

            RunScheduler().GetAwaiter().GetResult();

            Console.WriteLine("按任意键退出");
            Console.ReadKey();
        }

        private static async Task RunScheduler()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            await scheduler.Start();

            var job = JobBuilder.Create<HelloJob>()
                .StoreDurably()
                .Build();

            var trigger = TriggerBuilder.Create().StartAt(DateBuilder.DateOf(21, 22, 0))
                                        .WithSimpleSchedule(w => w.WithIntervalInSeconds(5)
                                                                .WithRepeatCount(20)
                                                                .WithMisfireHandlingInstructionNextWithExistingCount()
                                                                )
                                        .Build();

            await scheduler.AddJob(job, true);

            //await scheduler.ScheduleJob(job, trigger);


            var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
        }
    }

    public class HelloJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var fireTime = context.ScheduledFireTimeUtc?.ToLocalTime();
            var nextFireTime = context.NextFireTimeUtc?.ToLocalTime();
            var nowTime = DateTime.Now.ToLocalTime();
            await Console.Out.WriteLineAsync("now:" + nowTime + " ,fireTime:" + fireTime + ",nextFireTime:" + nextFireTime);
        }
    }
}
