﻿using log4net;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using System;
using System.Linq;
using System.Collections.Generic;
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
                    var prices = new List<float>();
                    for (int i = size - 1; i > 0 && i > size - 11; i--)
                    {
                        prices.Add(hist[i].Close);
                        historySeries.Points.Add(new DataPoint(10 - i, hist[i].Close));
                    }

                    if (Trend(prices)>0)
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


        public static double Trensd(List<float> prices)
        {

            float[] yVals = prices.ToArray();
            float[] xVals = Enumerable.Range(1, prices.Count).Select(x => x * 1f).ToArray();

            float sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double ssX = 0;
            double sumCodeviates = 0;
            double sCo = 0;
            double count = prices.Count-1;
            for (int ctr = 0; ctr < count; ctr++)
            {
                double x = xVals[ctr];
                double y = yVals[ctr];
                sumCodeviates += x * y;
            }
            ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            sCo = sumCodeviates - ((sumOfX * sumOfY) / count);
            return sCo / ssX;
        }

        public static float Trend(List<float> prices)
        {
            float[] xVals = Enumerable.Range(1, prices.Count).Select(x => x * 1f).ToArray();
            float[] yVals = prices.ToArray();

            float sumOfX = 0;
            float sumOfY = 0;
            float sumOfXSq = 0;
            float sumCodeviates = 0;

            for (var i = 0; i < xVals.Length; i++)
            {
                var x = xVals[i];
                var y = yVals[i];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
            }

            var count = xVals.Length;
            var ssX = sumOfXSq - ((sumOfX * sumOfX) / count);

            var sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            return sCo / ssX;
        }
    }
}

