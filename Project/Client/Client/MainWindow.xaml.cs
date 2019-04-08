using CommSystem;
using log4net;
using OxyPlot;
using OxyPlot.Series;
using Shared;
using Shared.Comms.ComService;
using Shared.MarketStructures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using static Client.Conversations.LeaderboardUpdate.ReceiveLeaderboardUpdateState;
using static Client.Conversations.StockUpdate.ReceiveStockUpdateState;

namespace Client
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
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

        //private void ReceivedLeaderboardUpdate(object sender, LeaderboardUpdateEventArgs e)
        //{

        //    // TODO: We need to decide how the records are going to be stored.
        //    //mem.HighScores = e.Records as SortedList<string,string>;
        //}

        public MainWindow()
        {
            Log.Debug($"{nameof(MainWindow)} (enter)");

            InitializeComponent();

            StockUpdateEventHandler += ReceivedStockUpdate;

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
            UpdateStockPanels();
        }

    }
}