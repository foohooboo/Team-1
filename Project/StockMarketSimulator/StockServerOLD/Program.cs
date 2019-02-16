using CommSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServer
{
    class Program
    {
        static void Main(string[] args)
        {
            HelloWorld helloWorld = new HelloWorld();
            Console.WriteLine("Hello World! From the stock Data Server.");
            Console.WriteLine(helloWorld.HelloText);
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }
    }
}
