using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace App.JobStore
{
    public class Example
    {
        public static async Task Run()
        {
            try
            {
                //NameValueCollection properties = new NameValueCollection();
                //properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
                //properties["quartz.jobStore.dataSource"] = "default";
                //properties["quartz.jobStore.tablePrefix"] = "QRTZ_";
                //properties["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz";
                //properties["quartz.dataSource.default.connectionString"] = "Database=QRTZ;Server=172.17.30.108;User ID=sa;Password=mingdao!@#123;Pooling=true;Max Pool Size=32767;Min Pool Size=0;";
                //properties["quartz.dataSource.default.provider"] = "SqlServer";
                //properties["quartz.serializer.type"] = "json";

                ISchedulerFactory sf = new StdSchedulerFactory();
                IScheduler sched = await sf.GetScheduler();

                DateTimeOffset startTime = DateBuilder.NextGivenSecondDate(null, 10);

                IJobDetail job1 = JobBuilder.Create<StateJob>()
                    .WithIdentity("job1", "group1")
                    .Build();
                job1.JobDataMap.Put(StateJob.Name, "A");
                job1.JobDataMap.Put(StateJob.Count, 1);

                ISimpleTrigger trigger1 = (ISimpleTrigger)TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartAt(startTime)
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).WithRepeatCount(500))
                    .Build();

                DateTimeOffset scheduleTime1 = await sched.ScheduleJob(job1, trigger1);
                Console.WriteLine($"{job1.Key} will run at: {scheduleTime1:r} and repeat: {trigger1.RepeatCount} times, every {trigger1.RepeatInterval.TotalSeconds} seconds");


                IJobDetail job2 = JobBuilder.Create<StateJob>()
                    .WithIdentity("job2", "group1")
                    .Build();
                job2.JobDataMap.Put(StateJob.Name, "B");
                job2.JobDataMap.Put(StateJob.Count, 1);

                ISimpleTrigger trigger2 = (ISimpleTrigger)TriggerBuilder.Create()
                    .WithIdentity("trigger2", "group1")
                    .StartAt(startTime)
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).WithRepeatCount(500))
                    .Build();

                DateTimeOffset scheduleTime2 = await sched.ScheduleJob(job2, trigger2);
                Console.WriteLine($"{job2.Key} will run at: {scheduleTime2:r} and repeat: {trigger2.RepeatCount} times, every {trigger2.RepeatInterval.TotalSeconds} seconds");

                //await sched.TriggerJob(new JobKey("job1", "group1"));
                //await sched.TriggerJob(new JobKey("job2", "group1"));

                await sched.Start();

                await Task.Delay(TimeSpan.FromMinutes(5));

                await sched.Shutdown(true);

                SchedulerMetaData metaData = await sched.GetMetaData();
                Console.WriteLine($"Executed {metaData.NumberOfJobsExecuted} jobs.");
            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }
    }
}
