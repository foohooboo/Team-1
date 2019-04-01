using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using CommSystem;
using log4net;
using Shared.MarketStructures;
using static Client.Conversations.LeaderboardUpdate.ReceiveLeaderboardUpdateState;
using static Client.Conversations.StockUpdate.ReceiveStockUpdateState;

namespace Client
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ManagedData mem = new ManagedData();

        public string StockCount { get; set; } = "0";//holds the data in buySell textbox


        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private HelloWorld helloWorld = new HelloWorld();
        public event PropertyChangedEventHandler PropertyChanged;

        private void ReceivedStockUpdate(object sender, StockUpdateEventArgs e)
        {
            mem.History.Add(e.CurrentDay);
        }

        private void ReceivedLeaderboardUpdate(object sender, LeaderboardUpdateEventArgs e)
        {

            // TODO: We need to decide how the records are going to be stored.
            //mem.HighScores = e.Records as SortedList<string,string>;
        }

        public MainWindow()
        {
            LeaderboardUpdateEventHandler += ReceivedLeaderboardUpdate;
            StockUpdateEventHandler += ReceivedStockUpdate;

            string method = "MainWindow Constructor";
            Log.Debug(String.Format("Enter - {0}", method));

            InitializeComponent();
            DataContext = this;

            helloWorld.HelloTextChanged += OnHelloTextChanged;
            HelloTextLocal = helloWorld.HelloText;

            Log.Debug(String.Format("Exit - {0}", method));


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
            stockPanels.Items.Clear();
            MarketDay day = mem.History[mem.History.Count - 1];
            foreach (ValuatedStock i in day.TradedCompanies)
            {
                Canvas shell = new Canvas
                {
                    Name = i.Symbol,
                    Height = 52,
                    Width = 183
                };
                Rectangle back = new Rectangle
                {
                    Height = 52,
                    Width = 183
                };
                TextBlock sym = new TextBlock
                {
                    Text = i.Symbol,
                    Margin = new Thickness(2)
                };
                TextBlock val = new TextBlock
                {
                    Text = i.Close.ToString("0.00"),
                    Width = 179,
                    TextAlignment = TextAlignment.Right
                };
                int qty = mem.MyPortfolio.GetAsset(i.Symbol).Quantity;


                shell.Children.Insert(0, back);
                shell.Children.Insert(1, sym);
                shell.Children.Insert(2, val);
                if (qty != 0)
                {
                    TextBlock amount = new TextBlock
                    {
                        Text = qty + " owned",
                        Margin = new Thickness(2, 20, 0, 0)
                    };
                    shell.Children.Insert(3, amount);
                }
                stockPanels.Items.Add(shell);

            }

        }

        public void OnHelloTextChanged(object source, EventArgs args)
        {
            string method = "OnHelloTextChanged";
            Log.Debug(String.Format("Enter - {0}", method));

            HelloTextLocal = helloWorld.HelloText;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HelloTextLocal"));

            Log.Debug(String.Format("Exit - {0}", method));
        }
        private void SendTransaction(int amount)
        {
            //TODO: This function should actually buy and sell stocks. Positive amount buys, negative sells
            //This function should check if the transaction is possible and if not will just do its best.
            //Instead of selling 100 it just sells what you have.
            //instead of buying 100 it just buys as much as your cash can afford.

            var selectedStock = stockPanels.SelectedItem as Canvas;
            if (selectedStock != null)
            {
                var symbol = (selectedStock.Children[1] as TextBlock).Text;
                HelloTextLocal = $"Initiated transaction for {amount} shares of {symbol}";
            }
            else
            {
                Log.Warn("No stocks selected, cannot send transaction.");
            }
            float value = 0;
            foreach (ValuatedStock i in mem.History[mem.History.Count - 1].TradedCompanies)
            {
                if (mem.SelectedAsset.RelatedStock.Name == i.Name)
                {
                    value = i.Close;
                    break;
                }
            }
            if (amount > 0 && mem.Cash < value * amount)
            {
                amount = (int)(mem.Cash / value);
            }
            else if (amount < 0 && -amount > mem.MyPortfolio.GetAsset(mem.SelectedAsset.RelatedStock.Symbol).Quantity)
            {
                amount = mem.MyPortfolio.GetAsset(mem.SelectedAsset.RelatedStock.Symbol).Quantity;
            }
            HelloTextLocal = amount.ToString() + " of " + mem.SelectedAsset.RelatedStock.Name;


        }
        private void SellOutEvent(object sender, RoutedEventArgs e)
        {
            SendTransaction(-mem.SelectedAsset.Quantity);

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
            mem.SelectedAsset = mem.MyPortfolio.GetAsset(((sender as ListBox).SelectedItem as Canvas).Name);
        }

        public void Button_Click_1(object sender, EventArgs e)
        {

        }

        private void Makeup_Click(object sender, RoutedEventArgs e)
        {
            mem.Cash = 100000;
            mem.History = ManagedData.makeupMarketSegment(15, 30);
            mem.MyPortfolio = ManagedData.makeupPortfolio(mem.History[0]);
            makeup.IsEnabled = false;
            updateStockPanels();
        }

    }
}