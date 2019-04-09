using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace CommSystem
{
    public class HelloWorld
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public HelloWorld()
        {
            Log.Debug($"{nameof(HelloWorld)} (enter)");

            _helloText = "Welcome to the Team-1 Stock Sim.     Good luck!";

            //Temporary Task...
            //Update the text from a "different source(thread)" in 10 seconds.
            //Used to ensure changes to this shared resource are reflected in each dependent project. 
            new Task(() =>
            {
                Thread.Sleep(10000);
                HelloText = "";
            }).Start();

            Log.Debug($"{nameof(HelloWorld)} (exit)");
        }

        //Example of a "Dependency Inversion" string property named "HelloText"
        private string _helloText;
        public delegate void HelloTextChangedHandler(object source, EventArgs args);
        public event HelloTextChangedHandler HelloTextChanged;
        public string HelloText
        {
            get => _helloText;
            set
            {
                if (_helloText != value)
                {
                    _helloText = value;
                    HelloTextChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
