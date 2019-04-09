using CommSystem;
using log4net;
using OxyPlot;
using OxyPlot.Series;
using Shared;
using Shared.Comms.ComService;
using Shared.Conversations;
using Shared.MarketStructures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Shared.Comms.Messages;
using Client.Conversations.LeaderboardUpdate;
using static Client.Conversations.LeaderboardUpdate.ReceiveLeaderboardUpdateState;
using static Client.Conversations.StockUpdate.ReceiveStockUpdateState;
using System.Collections;
using System.Collections.ObjectModel;

namespace Client
{
    public partial class MainWindow : Window, INotifyPropertyChanged, IHandleTraderModelChanged
    {
        public TraderModel TModel;

        public ObservableCollection<Leaders> LeaderBoard { get; set; } = new ObservableCollection<Leaders>();
        public ObservableCollection<AssetNetValue> ValueOfAssets { get; set; } = new ObservableCollection<AssetNetValue>();

        public ObservableCollection<StockButton> StockList { get; set; } = new ObservableCollection<StockButton>();

        public float TotalNetWorth { get; set; } = 12234234234.45f;

        public class Leaders
        {
            public string value { get; set; }
            public string name { get; set; }
        }

        public class StockButton
        {
            public string Symbol { get; set; }
            public int QtyOwned { get; set; }
            public float Price { get; set; }

            public StockButton(string symbol,int qtyOwned,float price)
            {
                Symbol = symbol;
                QtyOwned = qtyOwned;
                Price = Price;
            }
        }


        private ManagedData mem = new ManagedData();

        public string StockCount { get; set; } = "1";//holds the data in buySell textbox

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private HelloWorld helloWorld = new HelloWorld();
        public event PropertyChangedEventHandler PropertyChanged;

        private static Random rand = new Random();

        public PlotModel MyModel { get; private set; }

        private void ReceivedStockUpdate(object sender, StockUpdateEventArgs e)
        {
            mem.History.Add(e.CurrentDay);
        }

        public MainWindow(TraderModel model)
        {
            Log.Debug($"{nameof(MainWindow)} (enter)");

            InitializeComponent();

            TModel = model;
            TModel.Handler = this;
            this.Title = $"{TModel.Portfolio.Username}'s Portfolio.";

            var cash = TModel.Portfolio.Assets.Where(s => s.Key.Equals("$")).FirstOrDefault().Value;
            TModel.QtyCash = cash.Quantity;
            

            StockUpdateEventHandler += ReceivedStockUpdate;

            DataContext = this;

            helloWorld.HelloTextChanged += OnHelloTextChanged;
            HelloTextLocal = helloWorld.HelloText;

            GenerateDummyData();

            StockList.Add(new StockButton("AAPL",42,45.67f));
            StockList.Add(new StockButton("AMZN", 42, 32.1f));

            this.MyModel = new PlotModel { Title = "Selected Stock Name (SMBL)" };

            RedrawCandlestickChart(GenStockHistory());
            //ReDrawPortfolio();

            Log.Debug($"{nameof(MainWindow)} (exit)");
        }

        private static List<HighLowItem> GenStockHistory()
        {
            var hist = new List<HighLowItem>();

            //double x, double high, double low, double open = double.NaN, double close = double.NaN
            double open = GetClampedRandom(300, 500);
            for (int i = 1; i <= 30; i++)
            {
                double close = Clamp(open + GetClampedRandom(-50, 50), 10, 1000);
                double high = Math.Max(open, close) + GetClampedRandom(1, 50);
                double low = Math.Min(open, close) - GetClampedRandom(1, 50);

                hist.Add(new HighLowItem(i, high, low, open, close));

                open = Clamp(open + GetClampedRandom(-50, 50), 10, 1000);
            }

            return hist;
        }

        private void RedrawCandlestickChart(List<HighLowItem> newData) {
            var chart = new CandleStickSeries();
            chart.Items.AddRange(newData);
            MyModel.Series.Clear();
            MyModel.Series.Add(chart);
            MyModel.InvalidatePlot(true);
        }

        private static double GetClampedRandom(double min, double max)
        {
            return rand.NextDouble() * (max - min) + min;
        }

        private static double Clamp(double val, double min, double max)
        {
            if (val < min)
                val = min;
            if (val > max)
                val = max;
            return val;
        }

        ~MainWindow()
        {
            ComService.RemoveClient(Config.DEFAULT_UDP_CLIENT);
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
        public void UpdateStockPanels()
        {
            stockPanels.Items.Clear();
            MarketDay day = mem.History[mem.History.Count - 1];

            foreach (ValuatedStock i in day.TradedCompanies)
            {

                if (i.Symbol.Equals("$"))
                {
                    //TODO: Do something to display the user's cash value
                    //mem.Cash = i.Close;//Note: this is a awkward place to save the cash value but for now it is the only place.
                }
                else
                {
                    Canvas shell = new Canvas
                    {
                        Height = 52,
                        Width = 183,
                    };
                    Rectangle back = new Rectangle
                    {
                        Height = 52,
                        Width = 183
                    };
                    TextBlock sym = new TextBlock
                    {
                        Text = i.Symbol,
                        Padding = new Thickness(5, 5, 0, 0)
                    };
                    TextBlock val = new TextBlock
                    {
                        Text = i.Close.ToString("C0"),
                        Width = 179,
                        Padding = new Thickness(5),
                        TextAlignment = TextAlignment.Right
                    };

                    TextBlock amount = new TextBlock
                    {
                        Text = "",
                        Margin = new Thickness(5, 25, 0, 0)
                    };

                    int qty = mem.MyPortfolio.GetAsset(i.Symbol).Quantity;
                    if (qty > 0)
                    {
                        amount.Text = qty + " Owned";
                    }

                    shell.Children.Insert(0, back);
                    shell.Children.Insert(1, sym);
                    shell.Children.Insert(2, val);
                    shell.Children.Insert(3, amount);
                    
                    stockPanels.Items.Add(shell);
                }
            }
        }

        public void OnStockSelected(object sender, RoutedEventArgs e)
        {
            //TODO: move the logic that tracks what is selected to here instead of in the beginning of transaction.

            RedrawCandlestickChart(GenStockHistory());
        }

        public void OnHelloTextChanged(object source, EventArgs args)
        {
            HelloTextLocal = helloWorld.HelloText;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HelloTextLocal"));
        }

        private void SendTransaction(int amount)
        {
            //TODO: This function should actually buy and sell stocks. Positive amount buys, negative sells
            //This function should check if the transaction is possible and if not will just do its best.
            //Instead of selling 100 it just sells what you have.
            //instead of buying 100 it just buys as much as your cash can afford.

            var symbol = "";
            var selectedItem = stockPanels.SelectedItem;

            if (selectedItem == null)
            {
                //TODO: Move the following message to a better location, perhaps the place we will show error messages?
                HelloTextLocal = "You must select a stock item before attempting a transaction.";
                return;
            }

            var selectedStockCanvas = selectedItem as Canvas;
            symbol = (selectedStockCanvas.Children[1] as TextBlock).Text;

            var selectedVStock = mem.History[mem.History.Count - 1].TradedCompanies.SingleOrDefault(tc => tc.Symbol.Equals(symbol));
            if (selectedVStock != null)
            {
                float value = selectedVStock.Close;

                //buying
                if (amount > 0)
                {
                    if (mem.Cash < value * amount)
                    {
                        amount = (int)(mem.Cash / value);
                    }
                }
                //selling
                else
                {
                    if (-amount > mem.MyPortfolio.GetAsset(symbol).Quantity)
                    {
                        amount = -mem.MyPortfolio.GetAsset(symbol).Quantity;
                    }
                }
                HelloTextLocal = $"Initiated transaction for {amount} shares of {selectedVStock.Name} ({selectedVStock.Symbol}).";
            }
        }

        private void SellOutEvent(object sender, RoutedEventArgs e)
        {
            SendTransaction(int.MinValue + 1);

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
            SendTransaction(int.MaxValue);
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


            SendTransaction(int.Parse(StockCount));

        }

        private void SellEvent(object sender, RoutedEventArgs e)
        {
            SendTransaction(-int.Parse(StockCount));

        }

        public void Button_Click_1(object sender, EventArgs e)
        {

        }

        private void GenerateDummyData()
        {
            mem.Cash = 100000;
            mem.History = ManagedData.makeupMarketSegment(15, 30);
            mem.MyPortfolio = ManagedData.makeupPortfolio(mem.History[0]);
            //UpdateStockPanels();
        }

        public void ProfileChanged()
        {
            throw new NotImplementedException();
        }

        public void LeaderboardChanged()
        {
            LeaderBoard.Clear();
            SortedList<float,string> list = TraderModel.Current.Leaderboard;
            for (int i = list.Count - 1; i >= 0 && i > list.Count - 10; i--)
            {
                LeaderBoard.Add(new Leaders() { value = list.Keys[i].ToString("C0"), name = list.Values[i] });
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LeaderBoard"));
        }

        public void StockHistoryChanged()
        {
            throw new NotImplementedException();
        }

        public void ReDrawPortfolio()
        {
            //return;
            ValueOfAssets.Clear();
            float totalNetWorth = TraderModel.Current.QtyCash;

            //show cash first
            ValueOfAssets.Add(new AssetNetValue("CASH","", TraderModel.Current.QtyCash.ToString("C2")));

            //Clear qty owned for every item in stock list
            //foreach(var stockButton in stockPanels.Items)
            //{
                //
            //}

            //repopulate total net box
            var assets = TraderModel.Current.OwnedStocksByValue.Reverse();
            foreach(var asset in assets)
            {
                var symbol = asset.Value.RelatedStock.Symbol;
                //TODO: add qty owned for this item in stock list
                totalNetWorth += asset.Key;
                ValueOfAssets.Add(new AssetNetValue(symbol, asset.Value.Quantity.ToString(), asset.Key.ToString("C2")));
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ValueOfAssets"));
            TotalValueGridTextColumn.Header = totalNetWorth.ToString("C2");
        }

        public class AssetNetValue
        {
            public string Symbol { get; private set; }
            public string Quantity { get; private set; }
            public string TotalValue { get; private set; }

            public AssetNetValue(string symbol, string quantity, string totalValue)
            {
                Symbol = symbol;
                Quantity = quantity;
                TotalValue = totalValue;
            }
        }
    }
}