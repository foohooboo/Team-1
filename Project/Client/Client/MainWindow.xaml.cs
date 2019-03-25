using CommSystem;
using log4net;
using System;
using System.ComponentModel;
using System.Windows;
using Shared.MarketStructures;
using Shared.Portfolio;
using System.Collections.Generic;

namespace Client
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private float cash = 100000;
        private MarketSegment History;
        private Portfolio myPortfolio = new Shared.Portfolio.Portfolio();
        SortedList<string, string> HighScores =new SortedList<string, string>();
        private Shared.MarketStructures.Stock SelectedStock=new Stock("AAPL","Apple Inc.");
        private float MadeUpValue = 123;
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

        }
        private void SellOutEvent(object sender, RoutedEventArgs e)
        {
            SendTransaction(-myPortfolio.GetAsset(SelectedStock.Symbol).Quantity);
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

        }

        private void SellEvent(object sender, RoutedEventArgs e)
        {

        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}