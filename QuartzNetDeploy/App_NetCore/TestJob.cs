using Quartz;
using System;
using System.Threading.Tasks;

namespace App_NetCore
{
    [PersistJobDataAfterExecution]
    public class TestJob : IJob
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
