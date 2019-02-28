using CommSystem;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServer
{
    class CommSystemWrapper
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool WaitingForUpdate{get; set;}
        private HelloWorld helloWorld;

        public CommSystemWrapper(){
            Log.Debug($"{nameof(CommSystemWrapper)} (enter)");

            WaitingForUpdate = true;

            helloWorld = new HelloWorld();
            helloWorld.HelloTextChanged += OnHelloTextChanged;

            Log.Debug($"{nameof(CommSystemWrapper)} (exit)");
        }

        public string HelloText
        {
            get => helloWorld.HelloText;
            set{
                if (helloWorld.HelloText != value)
                {
                    helloWorld.HelloText = value;
                }
            }
        }
        
        public void OnHelloTextChanged(object source, EventArgs args)
        {
            Log.Debug($"{nameof(OnHelloTextChanged)} (enter)");

            WaitingForUpdate = false;

            Log.Debug($"{nameof(OnHelloTextChanged)} (exit)");
        }
    }
}
