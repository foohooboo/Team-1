using CommSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broker
{
    class CommSystemWrapper
    {
        public bool WaitingForUpdate{get; set;}
        private HelloWorld helloWorld;

        public CommSystemWrapper(){
            
            WaitingForUpdate = true;

            helloWorld = new HelloWorld();
            helloWorld.HelloTextChanged += OnHelloTextChanged;
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
            WaitingForUpdate = false;    
        }
    }
}
