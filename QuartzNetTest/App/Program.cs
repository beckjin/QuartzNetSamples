using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using System;
using System.Threading.Tasks;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            //LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

            RunScheduler().GetAwaiter().GetResult();

            Console.WriteLine("按任意键退出");
            Console.ReadKey();
        }

        private static async Task RunScheduler()
        {
            try
            {
                // 创建作业调度池
                ISchedulerFactory factory = new StdSchedulerFactory();
                IScheduler scheduler = await factory.GetScheduler();

                // 创建一个作业
                IJobDetail job = JobBuilder.Create<HelloJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                // 创建触发器，每10s执行一次
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(10)
                        .RepeatForever())
                    .Build();

                // 加入作业调度池中
                await scheduler.ScheduleJob(job, trigger);

                // 开始运行
                await scheduler.Start();
            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }

        private class ConsoleLogProvider : ILogProvider
        {
            public Logger GetLogger(string name)
            {
                return (level, func, exception, parameters) =>
                {
                    if (level >= LogLevel.Info && func != null)
                    {
                        Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
                    }
                    return true;
                };
            }

            public IDisposable OpenMappedContext(string key, string value)
            {
                throw new NotImplementedException();
            }

            public IDisposable OpenNestedContext(string message)
            {
                throw new NotImplementedException();
            }
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
            await Console.Out.WriteLineAsync("Hello Quartz.NET!");
        }
    }

  
}
