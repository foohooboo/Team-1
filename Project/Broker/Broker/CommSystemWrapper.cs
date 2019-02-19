using CommSystem;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broker
{
    class CommSystemWrapper
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool WaitingForUpdate{get; set;}
        private HelloWorld helloWorld;

        public CommSystemWrapper(){
            string method = "CommSystemWrapper Constructor";
            Log.Debug(string.Format("Enter - {0}", method));

            WaitingForUpdate = true;

            helloWorld = new HelloWorld();
            helloWorld.HelloTextChanged += OnHelloTextChanged;

            Log.Debug(string.Format("Exit - {0}", method));
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
            string method = "OnHelloTextChanged";
            Log.Debug(string.Format("Enter - {0}", method));

            WaitingForUpdate = false;

            Log.Debug(string.Format("Exit - {0}", method));
        }
    }
}
