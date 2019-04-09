using CommSystem;
using log4net;
using OxyPlot;
using OxyPlot.Series;
using Shared;
using Shared.Comms.ComService;
using Shared.MarketStructures;
using Shared.PortfolioResources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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

        public string StockCount { get; set; } = "1";//holds the data in buySell textbox

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private HelloWorld helloWorld = new HelloWorld();
        public event PropertyChangedEventHandler PropertyChanged;

        private static Random rand = new Random();

        public PlotModel MyModel { get; private set; }

        public MainWindow(TraderModel model)
        {
            Log.Debug($"{nameof(MainWindow)} (enter)");

            InitializeComponent();

            TModel = model;
            TModel.Handler = this;

            Title = $"{TModel.Portfolio.Username}'s Portfolio.";

            DataContext = this;

            helloWorld.HelloTextChanged += OnHelloTextChanged;
            HelloTextLocal = helloWorld.HelloText;

            this.MyModel = new PlotModel { Title = "Selected Stock Name (SMBL)" };

            ReDrawPortfolioItems();

            Log.Debug($"{nameof(MainWindow)} (exit)");
        }

        ~MainWindow()
        {
            ComService.RemoveClient(Config.DEFAULT_UDP_CLIENT);
        }

        private void RedrawCandlestickChart()
        {
            var history = TraderModel.Current.GetHistory(TraderModel.Current.SelectedStocksSymbol);

            if (history != null)
            {
                var chart = new CandleStickSeries();

                for (int i=0; i< history.Count; i++)
                {
                    chart.Items.Add(new HighLowItem(i+1,history[i].High, history[i].Low, history[i].Open, history[i].Close));
                }
                MyModel.Series.Clear();
                MyModel.Series.Add(chart);
                MyModel.InvalidatePlot(true);
                MyModel.ResetAllAxes();

                chart.XAxis.IsZoomEnabled = false;
                chart.YAxis.IsZoomEnabled = false;
            }
            else
            {
                MyModel.InvalidatePlot(false);
            }
        }   

        private string helloTextLocal; //Todo: change this to fading status field
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

        public void OnStockSelected(object sender, RoutedEventArgs e)
        {
            var selectedItem = stockPanels.SelectedItem as StockButton;

            if (selectedItem != null)
            {
                TraderModel.Current.SelectedStocksSymbol = selectedItem.Symbol;
            }

            RedrawCandlestickChart();
        }

        public void OnHelloTextChanged(object source, EventArgs args)
        {
            HelloTextLocal = helloWorld.HelloText; //TODO: break this out into a fading status process
        }

        /// <summary>
        /// Buy and sell stocks. Positive amount buys, negative sells. It clamps the desired amount to the
        /// largest possible amount if necessary.
        /// </summary>
        /// <param name="amount"></param>
        private void SendTransaction(int amount)
        {
            var symbol = TraderModel.Current.SelectedStocksSymbol;

            if (string.IsNullOrEmpty(symbol))
            {
                HelloTextLocal = "Please select a stock item before attempting a transaction.";
                return;
            }

            List<ValuatedStock> selectedVStockHistory = TraderModel.Current.GetHistory(symbol);

            if (selectedVStockHistory==null || selectedVStockHistory.Count == 0)
            {
                HelloTextLocal = $"We have not received a recent update for ({symbol}). Transaction canceled.";
                return;
            };

            var selectedVStock = selectedVStockHistory.Last();
            float value = selectedVStock.Close;

            //buying
            if (amount > 0)
            {
                var cash = TraderModel.Current.QtyCash;
                if (cash < value * amount)
                {
                    amount = (int)(cash / value);
                }
            }
            //selling
            else
            {
                Asset ownedAsset = null;
                var amountOwned = 0;

                if (TraderModel.Current.Portfolio.Assets.TryGetValue(symbol, out ownedAsset))
                {
                    amountOwned = ownedAsset.Quantity;
                }

                if (-amount > amountOwned)
                {
                    amount = -amountOwned;
                }
            }
            if (amount == 0)
            {
                HelloTextLocal = "Cannot perform the desired transaction.";
            }
            else
            {
                HelloTextLocal = $"Initiated transaction for {amount} shares of {selectedVStock.Name} ({selectedVStock.Symbol}).";

                //TODO: Start transaction request conversation.
                ReDrawPortfolioItems();
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

        public void ReDrawPortfolioItems()
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                ValueOfAssets.Clear();
                float totalNetWorth = TraderModel.Current.QtyCash;

                //show cash first
                ValueOfAssets.Add(new AssetNetValue("CASH", "", TraderModel.Current.QtyCash.ToString("C2")));

                //Clear stock list, then populate with owned stocks, followed by unowned
                StockList.Clear();

                //repopulate total net box and owned stocks in stocklist
                var assets = TraderModel.Current.OwnedStocksByValue;
                foreach (var asset in assets)
                {
                    var symbol = asset.RelatedStock.Symbol;
                    if (symbol.Equals("$")) continue;

                    var qtyOwned = asset.Quantity;

                    float price = TraderModel.Current.GetRecentValue(symbol);

                    StockList.Add(new StockButton(symbol, qtyOwned, price));

                    var assetNet = asset.Quantity * price;
                    totalNetWorth += assetNet;
                    ValueOfAssets.Add(new AssetNetValue(symbol, qtyOwned.ToString(), assetNet.ToString("C2")));
                }

                TotalValueGridTextColumn.Header = totalNetWorth.ToString("C2");

                //Populate unowned stocks in stock list
                if (TraderModel.Current.StockHistory?.Count > 0)
                {
                    foreach (var vStock in TraderModel.Current.StockHistory[0].TradedCompanies)
                    {
                        if (vStock.Symbol.Equals("$")) continue;

                        var stockButton = StockList.Where(s => s.Symbol.Equals(vStock.Symbol)).FirstOrDefault();

                        if (stockButton == null)
                        {
                            float price = TraderModel.Current.GetRecentValue(vStock.Symbol);
                            StockList.Add(new StockButton(vStock.Symbol, 0, price));
                        }
                    }
                }

                var selectedButton = StockList.Where(s => s.Symbol.Equals(TraderModel.Current.SelectedStocksSymbol)).FirstOrDefault();
                if (selectedButton != null)
                {
                    stockPanels.SelectedItem = selectedButton;
                }
            });   
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