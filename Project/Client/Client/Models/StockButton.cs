using log4net;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Client.Models
{
    public class StockButton
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Symbol { get; set; }
        public int QtyOwned { get; set; }
        public string Price { get; set; }
        public BitmapImage History { get; private set; }

        public StockButton(string symbol, int qtyOwned, float price)
        {
            Symbol = symbol;
            QtyOwned = qtyOwned;
            Price = price.ToString("C2");

            History = GenerateHistory(symbol);
        }

        //Note: Creating images is not the most efficient way to do this, 
        //but it was easy to implement. -Dsphar 4/10/2019
        private BitmapImage GenerateHistory(string symbol)
        {
            BitmapImage image = new BitmapImage(); ;

            try
            {
                var historySeries = new OxyPlot.Series.LineSeries
                {
                    LineStyle = LineStyle.Dash,
                };

                var hist = TraderModel.Current.GetHistory(symbol);
                var size = hist.Count;
                if (hist != null && size > 1)
                {
                    for (int i = size - 1; i > 0 && i > size - 11; i--)
                    {
                        historySeries.Points.Add(new DataPoint(10 - i, hist[i].Close));
                    }

                    if (hist[size - 1].Close > hist[size - 2].Close)
                        historySeries.Color = OxyColors.DarkGreen;
                    else
                        historySeries.Color = OxyColors.DarkRed;

                    PlotModel graph = new PlotModel
                    {
                        IsLegendVisible = false,
                        PlotAreaBorderThickness = new OxyThickness(0)
                    };
                    graph.Axes.Add(new OxyPlot.Axes.LinearAxis
                    {
                        Position = AxisPosition.Bottom,
                        IsAxisVisible = false
                    });
                    graph.Axes.Add(new OxyPlot.Axes.LinearAxis
                    {
                        Position = AxisPosition.Left,
                        IsAxisVisible = false
                    });


                    graph.Series.Add(historySeries);

                    var stream = new MemoryStream();
                    PngExporter.Export(graph, stream, 200, 52, OxyColors.Transparent);

                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return image;
        }
    }
}

