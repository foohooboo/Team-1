using log4net;
using StockServer.Data;
using System;
using System.Collections.Generic;

namespace StockServer
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        static void Main(string[] args)
        {
            Log.Debug($"{nameof(Main)} (enter)");

            StockData.Init();
            var comm = new CommSystemWrapper();

            Log.Info("Hello World! From the StockServer.");
            Log.Info(comm.HelloText);
            Log.Info("Now waiting for something to change shared resource value. Please wait...");

            while (comm.WaitingForUpdate)
            {

            }

            Log.Info(comm.HelloText);
            Log.Info("Something should have changed in the above text.");
            Log.Info("Pres any key to finish.");
            Console.ReadKey();

            Log.Debug($"{nameof(Main)} (exit)");
        }
        
        
    }
}
