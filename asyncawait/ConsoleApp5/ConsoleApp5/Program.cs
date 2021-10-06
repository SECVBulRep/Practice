using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("main start " + Thread.CurrentThread.ManagedThreadId);


            try
            {
                var t1 = new Task(DoWork);
                await t1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
         
            

            Console.WriteLine("main end " + Thread.CurrentThread.ManagedThreadId);
            Console.ReadKey();
        }

   


        public static async void DoWork()
        {
            Console.WriteLine("DoWork start " + Thread.CurrentThread.ManagedThreadId);

            await Task.Run(() => { Thread.Sleep(1000); });

            throw new Exception();
            
            Console.WriteLine("DoWork end " + Thread.CurrentThread.ManagedThreadId);
        }
    }
}