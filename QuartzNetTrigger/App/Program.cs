using Quartz;
using Quartz.Impl;
using Quartz.Impl.Calendar;
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

            // 排除一天中的时间范围不执行
            DailyCalendar dailyCalendar = new DailyCalendar(DateBuilder.DateOf(21, 0, 0).DateTime, DateBuilder.DateOf(22, 0, 0).DateTime);

            // 排除星期中的一天或多天
            WeeklyCalendar weeklyCalendar = new WeeklyCalendar();
            weeklyCalendar.SetDayExcluded(DayOfWeek.Friday, true);

            // 指定特定的日期，精确到天
            HolidayCalendar holidayCalendar = new HolidayCalendar();
            var holidayDateTime = new DateTime(2018, 11, 11);
            holidayCalendar.AddExcludedDate(holidayDateTime);

            // 排除月份中的某天，可选值为1-31，精确到天
            MonthlyCalendar monthlyCalendar = new MonthlyCalendar();
            monthlyCalendar.SetDayExcluded(31, true);

            // 排除每年中的某天，精确到天
            AnnualCalendar annualCalendar = new AnnualCalendar();
            var annualDateTime = new DateTime(2018, 11, 11);
            annualCalendar.SetDayExcluded(annualDateTime, true);

            // 使用表达式排除某些时间段不执行
            CronCalendar cronCalendar = new CronCalendar("* * * 24 3 ?");

            await scheduler.AddCalendar("calendar", cronCalendar, true, true);

            // trigger的附属信息
            var triggerDataMap = new JobDataMap();
            triggerDataMap.Add("name", "beck");

            // 创建触发器
            ITrigger trigger1 = TriggerBuilder.Create()
                .WithIdentity("trigger1", "triggerGroup1")
                .StartAt(DateBuilder.DateOf(18, 25, 40))
                .WithDailyTimeIntervalSchedule(w => w
                                    .WithRepeatCount(20)
                                    .WithIntervalInSeconds(4)
                                    .WithMisfireHandlingInstructionIgnoreMisfires())
                //.WithSimpleSchedule(w => w
                //                .WithRepeatCount(20)
                //                .WithIntervalInHours(1)
                //                .WithMisfireHandlingInstructionNowWithExistingCount())
                .UsingJobData(triggerDataMap)
                .WithPriority(3)
                .Build();

            // 创建触发器
            ITrigger trigger2 = TriggerBuilder.Create()
                .WithIdentity("trigger2", "triggerGroup1")
                .StartAt(startAt)
                .WithCronSchedule("* * * 24-25 3 ?")
                .ForJob(job)
                .UsingJobData(triggerDataMap)
                .ModifiedByCalendar("calendar")
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

            var fireTime = context.ScheduledFireTimeUtc?.ToLocalTime();
            var nextFireTime = context.NextFireTimeUtc?.ToLocalTime();
            var now = DateTime.Now.ToLocalTime();
            await Console.Out.WriteLineAsync($"triggerKeyName:{keyName}, now:{now}, fireTime: " + fireTime + ", nextFireTime: " + nextFireTime);
        }
    }
}
