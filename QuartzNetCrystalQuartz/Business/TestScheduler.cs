using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;

namespace Business
{
    public class TestScheduler
    {
        public static async Task<IScheduler> CreateScheduler()
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = await schedulerFactory.GetScheduler();

            var job = JobBuilder.Create<TestJob>()
                .WithIdentity("job1", "jobGroup")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "triggerGroup")
                .ForJob(job)
                .StartNow()
                .WithCronSchedule("0 /1 * ? * *")
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            await scheduler.Start();

            return scheduler;
        }
    }

    public class TestJob : IJob
    {
        private static readonly Random Random = new Random();

        public Task Execute(IJobExecutionContext context)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Greetings from HelloJob!");
            Console.ForegroundColor = color;

            return Task.Delay(TimeSpan.FromSeconds(Random.Next(1, 20)));
        }
    }
}
