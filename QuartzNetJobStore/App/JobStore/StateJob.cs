using Quartz;
using System;
using System.Threading.Tasks;

namespace App.JobStore
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class StateJob : IJob
    {
        public const string Name = "Name";
        public const string Count = "count";

        private int counter = 1;

        public async Task Execute(IJobExecutionContext context)
        {
            var key = context.JobDetail.Key;
            var data = context.JobDetail.JobDataMap;
            var name = data.GetString(Name);
            var count = data.GetInt(Count);
            count++;
            data.Put(Count, count);

            counter++;
            await Console.Out.WriteLineAsync(string.Format("Job: {0} executing at {1}, name is {2}, count (from job map) is {3}, count (from job member variable) is {4}",
                key,
                DateTime.Now.ToString("r"),
                name,
                count,
                counter));
        }
    }
}
