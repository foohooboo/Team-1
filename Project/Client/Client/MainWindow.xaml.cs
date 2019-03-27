using CommSystem;
using log4net;
using System;
using System.ComponentModel;
using System.Windows;
using Shared.MarketStructures;
using Shared.Portfolio;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Client
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        ManagedData mem = new ManagedData();
        public string StockCount { get; set; } = "0";//holds the data in textbox


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
                    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HelloTextLocal"));
                }
            }
        }
        public void updateStockPanels()
        {
            foreach (Canvas i in stockPanels.Items)
            {
                stockPanels.Items.Remove(i);
            }
            MarketDay day = mem.History[mem.History.Count];
            foreach(ValuatedStock i in day.TradedCompanies)
            {
                Canvas shell = new Canvas();
                shell.Height = 52;
                shell.Width = 183;
                Rectangle back = new Rectangle();
                back.Height = 52;
                back.Width = 183;
                TextBox sym = new TextBox();
                sym.Text =i.Symbol;
                shell.Children.Insert(0,back);
                shell.Children.Insert(1, sym);

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
        private void SendTransaction(int amount)
        {
            //TODO: This function should actually buy and sell stocks. Positive amount buys, negative sells
            //This function should check if the transaction is possible and if not will just do its best.
            //Instead of selling 100 it just sells what you have.
            //instead of buying 100 it just buys as much as your cash can afford.
            HelloTextLocal=amount.ToString();
        }
        private void SellOutEvent(object sender, RoutedEventArgs e)
        {
            try {
                Asset holder = mem.myPortfolio.GetAsset(mem.SelectedStock.Symbol);
                SendTransaction(-holder.Quantity);
            }
            catch { HelloTextLocal = mem.SelectedStock.Symbol + " not found in dictionary"; }
            
            
        }

        private void Sell100Event(object sender, RoutedEventArgs e)
        {
            SendTransaction(-100);
        }
        private void Sell10Event(object sender, RoutedEventArgs e)
        {
            SendTransaction(-10);
        }

        private void BuyOutEvent(object sender, RoutedEventArgs e)
        {
            SendTransaction(100000000);
        }

        private void Buy100Event(object sender, RoutedEventArgs e)
        {
            SendTransaction(100);
        }

        private void Buy10Event(object sender, RoutedEventArgs e)
        {
            SendTransaction(10);
        }

        private void BuyEvent(object sender, RoutedEventArgs e)
        {
            SendTransaction(Int32.Parse(StockCount));
            
        }

        private void SellEvent(object sender, RoutedEventArgs e)
        {
            SendTransaction(-Int32.Parse(StockCount));
           
        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}