using Quartz;
using Quartz.Impl;
using Quartz.Impl.Calendar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace App
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
            // 创建作业调度池
            //IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            var props = new NameValueCollection();
            props.Add("quartz.threadPool.ThreadCount", "20");

            //Environment.SetEnvironmentVariable("quartz.threadPool.ThreadCount", "22");

            ISchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            // 开始运行
            await scheduler.Start();

          
            var metaDate = await scheduler.GetMetaData();
            Console.WriteLine(metaDate.ThreadPoolSize);

            var start = DateBuilder.EvenSecondDateAfterNow();
            var end = DateBuilder.EvenSecondDateAfterNow().AddSeconds(10);

            // 创建触发器
            var jobDataMap = new JobDataMap();
            jobDataMap.Add("name", "beck");


            // 创建一个作业
            IJobDetail job = JobBuilder.Create<HelloJob>()
                .WithIdentity("job1", "group1")
                .UsingJobData(jobDataMap)
                .StoreDurably()
                .Build();

            // 创建触发器
            var trigger1DataMap = new JobDataMap();
            trigger1DataMap.Add("name", "trigger1");


            DailyCalendar dailyCalendar = new DailyCalendar(DateBuilder.DateOf(21, 0, 0).DateTime,
                                                           DateBuilder.DateOf(22, 0, 0).DateTime);

            WeeklyCalendar weeklyCalendar = new WeeklyCalendar();
            weeklyCalendar.SetDayExcluded(DayOfWeek.Friday, true);

            HolidayCalendar holidayCalendar = new HolidayCalendar();
            holidayCalendar.AddExcludedDate(DateTime.Now);

            MonthlyCalendar monthlyCalendar = new MonthlyCalendar();
            monthlyCalendar.SetDayExcluded(1, true);

            AnnualCalendar annualCalendar = new AnnualCalendar();
            annualCalendar.SetDayExcluded(DateTime.Now, true);

            CronCalendar cronCalendar = new CronCalendar("* * * 18 3 ?");

            await scheduler.AddCalendar("dailyCalendar", dailyCalendar, true, true);

            ITrigger trigger1 = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithPriority(2)
                .UsingJobData(trigger1DataMap)
                .StartAt(start)
                //.WithCronSchedule("0 0 0 ? * 4L")
                .WithDailyTimeIntervalSchedule(w => w.OnEveryDay()
                                                   .StartingDailyAt(new TimeOfDay(0, 28))
                                                   .EndingDailyAt(new TimeOfDay(22, 0))
                                                   .WithIntervalInSeconds(1)
                                                   .WithRepeatCount(5)
                                                   )
                //.WithCalendarIntervalSchedule(w => w.WithIntervalInWeeks(8))
                //.WithSchedule(SimpleScheduleBuilder.RepeatSecondlyForTotalCount(10))
                //.WithSimpleSchedule(x => x
                //    .WithIntervalInSeconds(1)
                //    .RepeatForever())
                //.EndAt(end)
                .Build();

            var trigger2DataMap = new JobDataMap();
            trigger2DataMap.Add("name", "trigger2");

            ITrigger trigger2 = TriggerBuilder.Create()
                .WithIdentity("trigger2", "group1")
                .WithPriority(4)
                .UsingJobData(trigger2DataMap)
                .ForJob(job)
                .StartAt(start)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(1)
                    .RepeatForever())
                .EndAt(end)
                .Build();

            var triggers = new List<ITrigger>();

            triggers.Add(trigger1);
            triggers.Add(trigger2);

            // 加入作业调度池中
            await scheduler.ScheduleJob(job, trigger1);

            // 删除trigger，可能会清掉job
            //await scheduler.UnscheduleJob(new TriggerKey("",""));
            // 暂停trigger
            //await scheduler.PauseTrigger(new TriggerKey("", ""));
            // 恢复trigger
            //await scheduler.ResumeTrigger(new TriggerKey("", ""));
            // 编辑trigger
            //await scheduler.RescheduleJob(new TriggerKey("", ""), trigger1);

            // 删除job
            //await scheduler.DeleteJob(new JobKey("", ""));
            // 暂停job
            //await scheduler.PauseJob(new JobKey("", ""));
            // 恢复job
            //await scheduler.ResumeJob(new JobKey("", ""));

            // 暂停scheduler
            //if (scheduler.IsStarted)
            //{
            //    await scheduler.Standby();
            //}
            // 重启scheduler
            //if (scheduler.InStandbyMode)
            //{
            //    await scheduler.Start();
            //}

            // 关闭scheduler，关闭后是不能重启的
            //await scheduler.Shutdown();

            // 编辑 trigger
            // trigger2.GetTriggerBuilder().ModifiedByCalendar("xxx");

            // 删除calendar,先解除和trigger的关系
            //await scheduler.DeleteCalendar("xxxx");

            // 多trigger
            //await scheduler.ScheduleJob(job, triggers, true);

            // trigger2 ForJob中的job必须已存在
            // await scheduler.ScheduleJob(trigger2);

            await Task.Delay(TimeSpan.FromMinutes(5));
            await scheduler.Shutdown(true);
        }
    }

    [PersistJobDataAfterExecution]
    //[DisallowConcurrentExecution]
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
            var name = context.JobDetail.JobDataMap["name"].ToString();
            //var name = context.MergedJobDataMap["name"].ToString();
            //Console.WriteLine(triggerName + "---" + jobName);

            context.JobDetail.JobDataMap["name"] = name + "1";

            var fireTime = context.ScheduledFireTimeUtc?.ToLocalTime();
            var nextFireTime = context.NextFireTimeUtc?.ToLocalTime();
            await Console.Out.WriteLineAsync("Hello " + name + ",fireTime:" + fireTime + ",nextFireTime:" + nextFireTime);

            Thread.Sleep(2000);
        }
    }
}
