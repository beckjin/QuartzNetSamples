using System;
using System.Threading;
using System.Threading.Tasks;

namespace App_NetCore
{
    class Program
    {
        private static readonly ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        private static TestSchedule testSchedule;

        static void Main(string[] args)
        {
            testSchedule = new TestSchedule();

            // 开启新线程执行 Schedule
            Task.Run(Start);

            Console.CancelKeyPress += (sender, e) =>
            {
                Task.Run(StopSilo);
            };

            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Task.Run(StopSilo);
            };

            // 进入阻塞状态，开始等待唤醒信号。（防止程序直接退出）
            manualResetEvent.WaitOne();
        }


        private static async Task Start()
        {
            await testSchedule.Start();
            Console.WriteLine("started");
        }

        private static async Task StopSilo()
        {
            await testSchedule.Stop();
            Console.WriteLine("stopped");
            // 取消阻塞状态，发出唤醒信号
            manualResetEvent.Set();
        }
    }
}
