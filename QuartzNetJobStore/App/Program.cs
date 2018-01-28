using App.JobStore;
using System;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            Example.Run().GetAwaiter().GetResult();
            Console.WriteLine("按任意键退出");
            Console.ReadKey();
        }
    }
}
