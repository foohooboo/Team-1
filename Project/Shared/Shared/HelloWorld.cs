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
            Log.Debug(string.Format("Enter - {0}", nameof(HelloWorld)));

            _helloText = "Hello from the shared CommSystem resource. ";

            //Temporary Task...
            //Update the text from a "different source(thread)" in 10 seconds.
            //Used to ensure changes to this shared resource are reflected in each dependent project. 
            new Task(() =>
            {
                Thread.Sleep(5000);
                HelloText += " !!!!!This text was updated by someone.!!!!!";
            }).Start();

            Log.Debug(string.Format("Exit - {0}", nameof(HelloWorld)));
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
