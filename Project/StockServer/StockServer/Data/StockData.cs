using Shared;
using System;
using System.Collections.Generic;

namespace StockServer.Data
{
    public class StockData
    {
        //TODO: Do we want to refactor this into a static class? -Dsphar 2/27/2019

        public readonly List<EvaluatedStocks> Data;

        public StockData()
        {
            Data = LoadStocksFromFile();
        }

        private List<EvaluatedStocks> LoadStocksFromFile()
        {
            /* To Add a new Stock: use this link and replace [SYMBOL] with a company's symbol
             * http://download.macrotrends.net/assets/php/stock_data_export.php?t=[SYMBOL]
             * Rename file to [SYMBOL].csv
             * Put file in StockServer/bin/HistoricData
             * Add tuple to hist[] below
            */
            int days = 1000;//Should be shorter than the shortest stock history.
            List<EvaluatedStocks> ret = new List<EvaluatedStocks>();//gets returned
            for (int i = 0; i < days; i++)
            {
                ret.Add(new EvaluatedStocks("2069-04-20"));
            }
            //TODO: It might be a good idea to have this loader try to parse every file in some directory.
            //That way we wont require any "magic" filenames like hist[,] below. The csv files themselves
            //could contain the company name and symbol. Not worth changing now, but maybe if we have time
            //in the future.     -Dsphar 2/27/2019
            string[,] hist = new string[,] {{"AAPL", "Apple Inc."}, {"AMZN", "Amazon.com Inc"}, {"BABA", "Alibaba Group"}
                , {"BAC", "Bank of America Corp." }, {"BUD", "Anheuser-Busch Inbev"}, {"CVX", "Chevron Corperation"}, {"FB", "Facebook Inc."}
                , {"GOOGL", "Alphabet"}, {"HD", "Home Depot Inc."}, {"JNJ", "Johnson & Johnson"}, {"JPM", "JPMorgan Chase & Co."}, { "MSFT", "Microsoft"}
                , { "PFE", "Pfizer Inc."}, {"PG", "Procter & Gamble Co."}, { "T", "AT&T Inc."}, { "UNH", "UnitedHealth Group Inc."}, { "V", "Visa Inc."}
                , {"WFC", "Wells Fargo & Co."}, { "WMT", "Walmart Inc."}, { "XOM", "ExxonMobil Corporation"}, { "GOOG", "Alphabet"}
            };

            List<Stock> stocks = new List<Stock>();
            Random random = new Random();
            int dates = random.Next(0, hist.GetLength(0));

            for (int i = 0; i < hist.GetLength(0); i++)
            {
                stocks.Add(new Stock(hist[i, 0], hist[i, 1]));
                List<EvaluatedStock> SingleStockUpdates = new List<EvaluatedStock>();
                float mult = (float)Math.Pow(2, (random.NextDouble() * 4 - 2));//gives nice range for multiplier
                using (var reader = new System.IO.StreamReader("HistoricData/" + hist[i, 0] + ".csv"))//open file based on symbol
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

                        SingleStockUpdates.Add(new EvaluatedStock(values, stocks[i]));
                    }
                    int l = SingleStockUpdates.Count;
                    int lr = random.Next(0, l - days);
                    SingleStockUpdates.RemoveRange(0, lr);
                    SingleStockUpdates.RemoveRange(days, SingleStockUpdates.Count - days);//use days long range and remove everything else
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
