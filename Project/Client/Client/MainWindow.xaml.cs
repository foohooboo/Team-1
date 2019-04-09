using CommSystem;
using log4net;
using OxyPlot;
using OxyPlot.Series;
using Shared;
using Shared.Comms.ComService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static Client.Conversations.StockUpdate.ReceiveStockUpdateState;

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
            public string Price { get; set; }

            public StockButton(string symbol, int qtyOwned, float price)
            {
                Symbol = symbol;
                QtyOwned = qtyOwned;
                Price = price.ToString("C2");
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

            StockList.Add(new StockButton("GOOG", 42, 45.67f));
            StockList.Add(new StockButton("AMZN", 42, 32.1f));
            StockList.Add(new StockButton("AAPL", 42, 150));

            TModel = model;
            TModel.Handler = this;

            ReDrawPortfolio();

            Title = $"{TModel.Portfolio.Username}'s Portfolio.";

            DataContext = this;

            helloWorld.HelloTextChanged += OnHelloTextChanged;
            HelloTextLocal = helloWorld.HelloText;

            GenerateDummyData();



            this.MyModel = new PlotModel { Title = "Selected Stock Name (SMBL)" };

            RedrawCandlestickChart(GenStockHistory());

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

        private void RedrawCandlestickChart(List<HighLowItem> newData)
        {
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
            SortedList<float, string> list = TraderModel.Current.Leaderboard;
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
            ValueOfAssets.Add(new AssetNetValue("CASH", "", TraderModel.Current.QtyCash.ToString("C2")));

            //Clear qty owned for every item in stock list
            foreach (StockButton s in StockList)
            {
                s.QtyOwned = 0;
            }

            //repopulate total net box
            var assets = TraderModel.Current.OwnedStocksByValue.Reverse();
            foreach (var asset in assets)
            {
                var symbol = asset.Value.RelatedStock.Symbol;
                if (symbol.Equals("$")) continue;

                var qtyOwned = asset.Value.Quantity;

                var stockButton = (StockList.Where(s => s.Symbol.Equals(symbol))).FirstOrDefault();
                if (stockButton == null)
                {
                    HelloTextLocal = $"Notice: You own stock in ({symbol}) which we haven't yet received data for.";
                }
                else
                {
                    stockButton.QtyOwned = qtyOwned;
                }

                totalNetWorth += asset.Key;
                ValueOfAssets.Add(new AssetNetValue(symbol, qtyOwned.ToString(), asset.Key.ToString("C2")));
            }
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ValueOfAssets"));
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