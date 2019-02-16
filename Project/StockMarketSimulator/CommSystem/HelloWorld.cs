using System;
using System.Collections.Generic;
using System.Text;

namespace CommSystem
{
    public class HelloWorld
    {
        static HelloWorld()
        {
            HelloText = "Hello from the shared Comm System resource. ";
        }

        //Example text property (for WPF binding)
        private static string helloText;
        private static event EventHandler HelloTextChanged = (sender, e) => { return;};
        public static string HelloText
        {
            get => helloText;
            set
            {
                if (helloText == value)
                    return;
                helloText = value;
                HelloTextChanged?.Invoke(null, EventArgs.Empty);
            }
        }
        
        
        
    }
}
