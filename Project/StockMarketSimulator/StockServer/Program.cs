using System;

namespace StockServer
{
    class Program
    {

        static void Main(string[] args)
        {
            var comm = new CommSystemWrapper();

            Console.WriteLine("Hello World! From the StockServer.");
            Console.WriteLine(comm.HelloText);
            Console.WriteLine("Now waiting for something to change shared resource value. Please wait...");

            while (comm.WaitingForUpdate)
            {

            }

            Console.WriteLine(comm.HelloText);
            Console.WriteLine("Something should have changed in the above text.");
            Console.WriteLine("Pres any key to finish.");
            Console.ReadKey();
        }
    }
}
