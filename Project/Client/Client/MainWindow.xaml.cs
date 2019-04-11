using Client.Conversations;
using Client.Models;
using log4net;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Shared;
using Shared.Comms.ComService;
using Shared.Conversations;
using Shared.MarketStructures;
using Shared.PortfolioResources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public partial class MainWindow : Window, INotifyPropertyChanged, IHandleTraderModelChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public ObservableCollection<Leaders> LeaderBoard { get; set; } = new ObservableCollection<Leaders>();
        public ObservableCollection<AssetNetValue> ValueOfAssets { get; set; } = new ObservableCollection<AssetNetValue>();
        public ObservableCollection<StockButton> StockList { get; set; } = new ObservableCollection<StockButton>();
        public static string StockCount { get; set; } = "1";//holds the data in buySell textbox

        public event PropertyChangedEventHandler PropertyChanged;

        private string _notification = "Welcome to Team-1's Stock Trading Simulation.   Good luck!!";
        public string Notification
        {
            get => _notification;
            set
            {
                _notification = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Notification"));
                LastNotification = DateTime.Now;
            }
        }
        private object LockCleanNotify = new object();
        private bool DoCleanNotify = true;
        private DateTime LastNotification = DateTime.Now;

        public PlotModel CandlestickView { get; private set; } = new PlotModel { TitleColor = OxyColors.White, IsLegendVisible = false };

        public MainWindow()
        {
            Log.Debug($"{nameof(MainWindow)} (enter)");

            InitializeComponent();
            DataContext = this;

            TraderModel.Current.Handler = this;
            Title = $"{TraderModel.Current.Portfolio.Username}'s Portfolio.";

            //start notification monitor
            Task.Run(() =>
            {
                var clean = true;
                while (clean)
                {
                    double ellapsed;
                    lock (LockCleanNotify)
                    {
                        ellapsed = (DateTime.Now - LastNotification).TotalSeconds;
                        clean = DoCleanNotify;
                    }
                    if (ellapsed > 3.5)
                    {
                        _notification = "";
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Notification"));
                    }
                    Thread.Sleep(500);
                }
            });

            CandlestickView.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                IsZoomEnabled = false,
                IsAxisVisible = false
            });

            CandlestickView.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                IsZoomEnabled = false,
                MajorGridlineColor = OxyColor.FromRgb(40, 40, 40),
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineColor = OxyColor.FromRgb(32, 32, 32),
                MinorGridlineStyle = LineStyle.LongDash,
                TextColor = OxyColor.FromRgb(128, 128, 128),
                TitleColor = OxyColors.White,
                AxisTitleDistance = 15,
                Title = "US Dollars"
            });

            ReDrawPortfolioItems();

            Log.Debug($"{nameof(MainWindow)} (exit)");
        }

        ~MainWindow()
        {
            ComService.RemoveClient(Config.DEFAULT_UDP_CLIENT);

            lock (LockCleanNotify)
            {
                DoCleanNotify = false;
            }
        }

        private void RedrawStockCharts()
        {
            var symbol = TraderModel.Current.SelectedStocksSymbol;
            var history = TraderModel.Current.GetHistory(TraderModel.Current.SelectedStocksSymbol);

            if (history != null && history.Count > 0)
            {
                CandlestickView.Title = $"{symbol}  --  {history[0].Name}";
                RedreawCandlestick(history);
                RedreawVolume(history);
            }
            else
            {
                CandlestickView.Title = null;
                CandlestickView.InvalidatePlot(false);
            }
        }

        private void RedreawCandlestick(List<ValuatedStock> history)
        {
            if (history != null && history.Count > 0)
            {
                var candlestickChart = new CandleStickSeries
                {

                };

                for (int i = 0; i < history.Count; i++)
                {
                    candlestickChart.Items.Add(new HighLowItem(i + 1, history[i].High, history[i].Low, history[i].Open, history[i].Close));
                }

                CandlestickView.Series.Clear();
                CandlestickView.Series.Add(candlestickChart);
                CandlestickView.InvalidatePlot(true);
                CandlestickView.ResetAllAxes();
            }
            else
            {
                CandlestickView.InvalidatePlot(false);
            }
        }

        private void RedreawVolume(List<ValuatedStock> history)
        {
            //Note: There seems to be an oxyplot rendering bug when updating ColumnSeries via MVVM. As such, I have to push
            //a whole new PlotModel every time. But hey, at least it works... -Dsphar 4/9/2019

            var series = new ColumnSeries();

            if (history != null && history.Count > 0)
            {
                float max = history.Max(x => x.Volume);
                float min = history.Min(x => x.Volume) - (0.05f * max); //Remove en extra 1% of max, so no % is actually 0.
                float spread = max - min;

                for (int i = 0; i < history.Count; i++)
                {
                    float adjusted = history[i].Volume - min;
                    float fraction = adjusted / spread;
                    float percent = fraction * 100;

                    var col = new ColumnItem(percent);

                    if (history[i].Close > history[i].Open)
                        col.Color = OxyColors.Green;
                    else
                        col.Color = OxyColors.Red;

                    series.Items.Add(col);
                }

                var model = new PlotModel();

                model.Axes.Add(new CategoryAxis
                {
                    Position = AxisPosition.Left,
                    IsZoomEnabled = false,

                    MajorGridlineColor = OxyColors.Black,
                    MajorGridlineStyle = LineStyle.Solid,

                    TextColor = OxyColor.FromRgb(24, 24, 24),
                    AxisTitleDistance = 15,

                    TitleColor = OxyColors.White,
                    Title = "Relative Volume"
                });
                model.Axes.Add(new CategoryAxis
                {
                    Position = AxisPosition.Bottom,
                    IsZoomEnabled = false,
                    IsAxisVisible = false,
                });
                model.Series.Add(series);
                VolumeChart.Model = model;
            }
            else
            {
                CandlestickView.InvalidatePlot(false);
            }
        }

        public void OnStockSelected(object sender, RoutedEventArgs e)
        {
            var selectedItem = stockPanels.SelectedItem as StockButton;
            if (selectedItem != null)
            {
                TraderModel.Current.SelectedStocksSymbol = selectedItem.Symbol;
            }
            RedrawStockCharts();
        }

        /// <summary>
        /// Buy and sell stocks. Positive amount buys, negative sells. It clamps the desired amount to the
        /// largest possible amount if necessary.
        /// </summary>
        /// <param name="amount"></param>
        private void SendTransaction(float amount)
        {
            var symbol = TraderModel.Current.SelectedStocksSymbol;

            if (string.IsNullOrEmpty(symbol))
            {
                Notification = "Please select a stock item before attempting a transaction.";
                return;
            }

            List<ValuatedStock> selectedVStockHistory = TraderModel.Current.GetHistory(symbol);

            if (selectedVStockHistory == null || selectedVStockHistory.Count == 0)
            {
                Notification = $"We have not received a recent update for ({symbol}). Transaction canceled.";
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
                var amountOwned = 0f;

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
                Notification = "Cannot perform the desired transaction.";
            }
            else
            {
                Notification = $"Initiated transaction for {amount} shares of {selectedVStock.Name} ({selectedVStock.Symbol}).";

                //Prepare and send transaction request
                var stock = new ValuatedStock()
                {
                    Symbol = symbol,
                    Close = TraderModel.Current.GetRecentValue(symbol)
                };
                var requestConv = new InitiateTransactionConversation(TraderModel.Current.Portfolio.PortfolioID);

                requestConv.SetInitialState(new InitTransactionStartingState(requestConv, stock, amount));
                ConversationManager.AddConversation(requestConv);

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

                RedrawStockCharts();

                var currentBtn = 0;

                //Update Net Asset box
                ValueOfAssets.Clear();
                float totalNetWorth = TraderModel.Current.QtyCash;
                ValueOfAssets.Add(new AssetNetValue("CASH", "", TraderModel.Current.QtyCash.ToString("C2")));
                var assets = TraderModel.Current.OwnedStocksByValue;
                foreach (var asset in assets)
                {
                    var symbol = asset.RelatedStock.Symbol;
                    if (symbol.Equals("$")) continue;
                    var qtyOwned = asset.Quantity;
                    float price = TraderModel.Current.GetRecentValue(symbol);
                    var assetNet = asset.Quantity * price;
                    totalNetWorth += assetNet;

                    ValueOfAssets.Add(new AssetNetValue(symbol, qtyOwned.ToString(), assetNet.ToString("C2")));
                }
                TotalValueGridTextColumn.Header = totalNetWorth.ToString("C2");

                //Update owned stocks in StockList
                assets = TraderModel.Current.OwnedStocksBySymbol;
                foreach (var asset in assets)
                {
                    var symbol = asset.RelatedStock.Symbol;
                    if (symbol.Equals("$")) continue;

                    var qtyOwned = asset.Quantity;

                    float price = TraderModel.Current.GetRecentValue(symbol);

                    StockList.Insert(currentBtn, new StockButton(symbol, qtyOwned, price));
                    currentBtn++;
                    if (currentBtn < StockList.Count)
                        StockList.RemoveAt(currentBtn);
                }

                //Update unowned stocks in StockList
                if (TraderModel.Current.StockHistory?.Count > 0)
                {
                    foreach (var vStock in TraderModel.Current.StockHistory[0].TradedCompanies)
                    {
                        if (vStock.Symbol.Equals("$")) continue;
                        var stockButton = assets.Where(s => s.RelatedStock.Symbol.Equals(vStock.Symbol)).FirstOrDefault();
                        if (stockButton == null)
                        {
                            float price = TraderModel.Current.GetRecentValue(vStock.Symbol);
                            StockList.Insert(currentBtn, new StockButton(vStock.Symbol, 0, price));
                            currentBtn++;
                            if (currentBtn < StockList.Count)
                                StockList.RemoveAt(currentBtn);
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

        public void ShowStatus(string message)
        {
            Notification = message;
        }
    }
}