using log4net;
using Shared.MarketStructures;
using System;
using System.Collections.Generic;

namespace StockServer.Data
{
    public class StockData
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static MarketSegment _fullHistory = null;
        private static MarketSegment FullHistory
        {
            get
            {
                if (_fullHistory == null)
                    Init();
                return _fullHistory;
            }
            set { }
        }

        private static int _currentDayNumber;
        public static int CurrentDayNumber
        {
            get
            {
                if (_fullHistory == null)
                    return 0;
                else
                    return _currentDayNumber;
            }
            private set
            {
                _currentDayNumber = value % FullHistory.Count;
            }
        }

        public static void Init()
        {
            Log.Debug($"{nameof(Init)} (enter)");

            if (_fullHistory == null)
                _fullHistory = LoadStocksFromFile();

            CurrentDayNumber = 0;

            Log.Debug($"{nameof(Init)} (exit)");
        }

        public static int GetSize()
        {
            return FullHistory.Count;
        }

        public static MarketSegment GetRecentHistory(int numDays)
        {
            int currentIndex = CurrentDayNumber - numDays; ;
            if (currentIndex < 0)
                currentIndex = (currentIndex + GetSize()+1) % GetSize();//account for rollover

            var segment = new MarketSegment();
            for(int i=0; i<numDays; i++)
            {
                
                segment.Add(FullHistory[currentIndex]);
                currentIndex = (++currentIndex) % GetSize();
            }

            return segment;
        }

        public static MarketDay GetCurrentDay()
        {
            return FullHistory[CurrentDayNumber];
        }

        public static MarketDay AdvanceDay()
        {
            CurrentDayNumber++;
            return GetCurrentDay();
        }

        private static MarketSegment LoadStocksFromFile()
        {
            /* To Add a new Stock: use this link and replace [SYMBOL] with a company's symbol
             * http://download.macrotrends.net/assets/php/stock_data_export.php?t=[SYMBOL]
             * Rename file to [SYMBOL].csv
             * Put file in StockServer/bin/HistoricData
             * Add tuple to hist[] below
            */

            //TODO: If we ever remove the Hist[,] below, we will probably also want to remove this magic "days" number.
            //It might be a pain, but one way is to step every file and just count the number of days, finding the smallest
            //and setting this value to that. That way the files can live completely on their own and users can change them
            //to any size without having to re-compile. -Dsphar 3/1/2019
            int days = 1000;//Should be shorter than the shortest stock history.


            MarketSegment ret = new MarketSegment();//gets returned
            DateTime date = DateTime.Now;
            date = date.AddYears(50);
            for (int i = 0; i < days; i++)
            {
                date = date.AddDays(1);
                if ((date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday))
                {
                    date = date.AddDays(1);
                }
                ret.Add(new MarketDay(date.ToString("yyyy-MM-dd")));
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
                List<ValuatedStock> SingleStockUpdates = new List<ValuatedStock>();
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

                        SingleStockUpdates.Add(new ValuatedStock(values, stocks[i]));
                    }
                    int l = SingleStockUpdates.Count;
                    int lr = random.Next(0, l - days);
                    SingleStockUpdates.RemoveRange(0, lr);
                    SingleStockUpdates.RemoveRange(days, SingleStockUpdates.Count - days);//use days long range and remove everything else
                    for (int j = 0; j < days; j++)
                    {
                        //multiply values by the same number for a whole stock history so its hard to cheat.
                        SingleStockUpdates[j].Close = (float)Math.Round((double)(SingleStockUpdates[j].Close * mult), 2);
                        SingleStockUpdates[j].Open = (float)Math.Round((double)(SingleStockUpdates[j].Open * mult), 2);
                        SingleStockUpdates[j].High = (float)Math.Round((double)(SingleStockUpdates[j].High * mult), 2);
                        SingleStockUpdates[j].Low = (float)Math.Round((double)(SingleStockUpdates[j].Low * mult), 2);
                        SingleStockUpdates[j].Volume = (int)((float)SingleStockUpdates[j].Volume / mult);
                        ret[j].Data.Add(SingleStockUpdates[j]);
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// GetFullHistory is a hack to allow unit testing to get PrivateObject access to the FullHistory,
        /// property. It can probably be cleaned up somehow, but I am ready to move on.
        /// Note: it should only to be used for confirmation purposes.   -Dsphar 3/2/2019
        /// </summary>
        /// <returns></returns>
        private MarketSegment GetFullHistory()
        {
            return FullHistory;
        }
    }
}
