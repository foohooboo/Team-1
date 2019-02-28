using log4net;
using System;
using System.Collections.Generic;

namespace StockServer
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        static void Main(string[] args)
        {
            List<Shared.EvaluatedStocks> master = LoadStocksFromFile();

            string method = "Main";
            Log.Debug(string.Format("Enter - {0}", method));
            Log.Info("Loading Stock Data");

            var comm = new CommSystemWrapper();
            

            Log.Info("Hello World! From the StockServer.");
            Log.Info(comm.HelloText);
            Log.Info("Now waiting for something to change shared resource value. Please wait...");

            while (comm.WaitingForUpdate)
            {

            }

            Log.Info(comm.HelloText);
            Log.Info("Something should have changed in the above text.");
            Log.Info("Pres any key to finish.");
            Console.ReadKey();

            Log.Debug(string.Format("Exit - {0}", method));
        }

        static public List<Shared.EvaluatedStocks> LoadStocksFromFile()
        {
            /* To Add a new Stock: use this link and replce [SYMBOL] with a companies symbol
             * http://download.macrotrends.net/assets/php/stock_data_export.php?t=[SYMBOL]
             * Rename file to [SYMBOL].csv
             * Put file in StockServer/bin/HistoricData
             * Add tuple to hist[] below
            */
            int days = 1000;//Should be shorter than the shortest stock history.
            List<Shared.EvaluatedStocks> ret = new List<Shared.EvaluatedStocks>();//gets returned
            for(int i = 0; i < days; i++)
            {
                ret.Add(new Shared.EvaluatedStocks("2069-04-20"));
            }
            string[,] hist = new string[,] {{"AAPL", "Apple Inc."}, {"AMZN", "Amazon.com Inc"}, {"BABA", "Alibaba Group"}
                , {"BAC", "Bank of America Corp." }, {"BUD", "Anheuser-Busch Inbev"}, {"CVX", "Chevron Corperation"}, {"FB", "Facebook Inc."}
                , {"GOOGL", "Alphabet"}, {"HD", "Home Depot Inc."}, {"JNJ", "Johnson & Johnson"}, {"JPM", "JPMorgan Chase & Co."}, { "MSFT", "Microsoft"}
                , { "PFE", "Pfizer Inc."}, {"PG", "Procter & Gamble Co."}, { "T", "AT&T Inc."}, { "UNH", "UnitedHealth Group Inc."}, { "V", "Visa Inc."}
                , {"WFC", "Wells Fargo & Co."}, { "WMT", "Walmart Inc."}, { "XOM", "ExxonMobil Corporation"}, { "GOOG", "Alphabet"}
            };
            
            List<Shared.Stock> stocks = new List<Shared.Stock>();
            Random random = new Random();
            int dates=random.Next(0, hist.GetLength(0));

            for (int i = 0; i < hist.GetLength(0); i++)
            {
                stocks.Add(new Shared.Stock(hist[i,0], hist[i,1]));
                List<Shared.EvaluatedStock> SingleStockUpdates = new List<Shared.EvaluatedStock>();
                float mult = (float)Math.Pow(2, (random.NextDouble() * 4 - 2));//gives nice range for multiplier
                using (var reader = new System.IO.StreamReader("..\\..\\HistoricData\\" + hist[i,0] + ".csv"))//open file based on symbol
                {
                    
                    for (int j = 0; j < 11; j++) { reader.ReadLine(); }//skip the first 11 lines
                    //Read in a stock, for all time,
                    //randomly pick n days in order
                    //multiply by random constant
                    //TODO: Randomize Name and Symbol
                    //build up ret
                    while (!reader.EndOfStream)
                    {

                        string line = reader.ReadLine();
                        string[] values = line.Split(',');
                        //Format: string[date,open,high,low,close,volume]
                        
                        
                        SingleStockUpdates.Add(new Shared.EvaluatedStock(values,stocks[i]));
                    }
                    int l=SingleStockUpdates.Count;
                    int lr = random.Next(0, l - days);
                    SingleStockUpdates.RemoveRange(0, lr);
                    SingleStockUpdates.RemoveRange(days, SingleStockUpdates.Count-days);//use days long range and remove everything else
                    for (int j = 0; j < days; j++)
                    {
                        //multiply values by the same number for a whole stock history so its hard to cheat.
                        SingleStockUpdates[j].close = (float)Math.Round((double)(SingleStockUpdates[j].close * mult), 2);
                        SingleStockUpdates[j].open = (float)Math.Round((double)(SingleStockUpdates[j].open * mult), 2);
                        SingleStockUpdates[j].high = (float)Math.Round((double)(SingleStockUpdates[j].high * mult), 2);
                        SingleStockUpdates[j].low = (float)Math.Round((double)(SingleStockUpdates[j].low * mult), 2);
                        SingleStockUpdates[j].volume = (int)((float)SingleStockUpdates[j].volume / mult);
                        ret[i].Add(SingleStockUpdates[j]);
                        
                    }
                    
                }
            }
            return ret;
        }
    }
}
