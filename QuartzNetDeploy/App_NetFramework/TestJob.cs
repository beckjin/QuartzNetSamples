using App_NetFramework.Log;
using Quartz;
using System;
using System.Threading.Tasks;

namespace App_NetFramework
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

            Log4NetHelper.Instance.Info($"execute {times} times");

            await Task.CompletedTask;
        }
    }
}
