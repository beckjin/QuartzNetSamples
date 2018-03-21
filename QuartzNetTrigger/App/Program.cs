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

            // 指定特定的日期，精度到天
            HolidayCalendar holidayCalendar = new HolidayCalendar();
            var holidayDateTime = new DateTime(2018, 3, 1);
            holidayCalendar.AddExcludedDate(holidayDateTime);

            // 排除月份中的某天，可选值为1-31，精度到天
            MonthlyCalendar monthlyCalendar = new MonthlyCalendar();
            monthlyCalendar.SetDayExcluded(1, true);

            // 排除每年中的某天，精度到天
            AnnualCalendar annualCalendar = new AnnualCalendar();
            var annualDateTime = new DateTime(2018, 3, 1);
            annualCalendar.SetDayExcluded(annualDateTime, true);

            // 使用表达式排除某些时间段不执行
            CronCalendar cronCalendar = new CronCalendar("* * * 18 3 ?");

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
