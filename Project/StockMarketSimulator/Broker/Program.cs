using CommSystem;
using System;

namespace Broker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! From the Broker Server.");
            Console.WriteLine(HelloWorld.HelloText);
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }
    }
}
