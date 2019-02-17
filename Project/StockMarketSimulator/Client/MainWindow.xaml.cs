using CommSystem;
using log4net;
using System;
using System.ComponentModel;
using System.Windows;

namespace Client
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private HelloWorld helloWorld = new HelloWorld();
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            string method = "MainWindow Constructor";
            Log.Debug(string.Format("Enter - {0}", method));

            InitializeComponent();
            DataContext = this;

            helloWorld.HelloTextChanged += OnHelloTextChanged;
            HelloTextLocal = helloWorld.HelloText;

            Log.Debug(string.Format("Exit - {0}", method));
        }

        private string helloTextLocal;
        public string HelloTextLocal
        {
            get => helloTextLocal;
            set
            {
                if (helloTextLocal != value)
                {
                    helloTextLocal = value;
                    helloWorld.HelloText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HelloTextLocal"));
                }
            }
        }

        public void OnHelloTextChanged(object source, EventArgs args)
        {
            string method = "OnHelloTextChanged";
            Log.Debug(string.Format("Enter - {0}", method));

            HelloTextLocal = helloWorld.HelloText;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HelloTextLocal"));

            Log.Debug(string.Format("Exit - {0}", method));
        }
    }
}