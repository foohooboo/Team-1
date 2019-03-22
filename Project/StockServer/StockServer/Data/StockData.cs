using log4net;
using Shared.MarketStructures;
using System;
using System.Collections.Generic;
using System.IO;

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

        public static void Init()
        {
            Log.Debug($"{nameof(Init)} (enter)");

            if (_fullHistory == null)
                LoadStocksFromFile();

            CurrentDayNumber = 0;

            Log.Debug($"{nameof(Init)} (exit)");
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

        public static int GetSize()
        {
            return FullHistory.Count;
        }

        public static MarketSegment GetRecentHistory(int numDays)
        {
            int currentIndex = CurrentDayNumber - numDays; ;
            if (currentIndex < 0)
                currentIndex = (currentIndex + GetSize() + 1) % GetSize();//account for rollover

            var segment = new MarketSegment();
            for (int i = 0; i < numDays; i++)
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

        private static DateTime NextWeekday(DateTime start)
        {
            var day = start.AddDays(1);
            while ((day.DayOfWeek == DayOfWeek.Saturday) || (day.DayOfWeek == DayOfWeek.Sunday))
            {
                day = day.AddDays(1);
            }
            return day;
        }

        private static void LoadStocksFromFile()
        {
            if (_fullHistory != null)
            {
                throw new Exception("Cannot LoadStocksFromFile more than once.");
            }

            _fullHistory = new MarketSegment();
            var random = new Random();
            int range = 15 * 365;
            DateTime coverDate = DateTime.Today.AddDays(random.Next(range));
            coverDate = NextWeekday(coverDate);
            float mult = (float)Math.Pow(2, (random.NextDouble() * 4 - 2));//gives nice range for multiplier

            //Import all .csv files into companiesRaw
            var companiesRaw = new List<CompanyDataRaw>();
            foreach (string filename in Directory.EnumerateFiles("HistoricData", "*.csv"))
            {
                using (var fileReader = new StreamReader(filename))
                {
                    var currentCompany = new CompanyDataRaw();

                    var symbol = fileReader.ReadLine().Trim('"');
                    var name = fileReader.ReadLine().Trim('"');
                    currentCompany.MetaData = new Stock(symbol, name);

                    //Prepare filestream
                    fileReader.ReadLine();
                    fileReader.ReadLine();
                    fileReader.ReadLine();
                    while (!fileReader.EndOfStream)
                    {
                        currentCompany.DailyValue.Add(fileReader.ReadLine());
                    }
                    companiesRaw.Add(currentCompany);
                }
            }

            //Cache stock data (only when all files have data for the given day)
            var keepParsing = true;
            for (int dayIndex = 0; dayIndex < companiesRaw[0].DailyValue.Count - 1 && keepParsing; dayIndex++)
            {
                DateTime currentDate = companiesRaw[0].CurrentDate;
                foreach (var company in companiesRaw)
                {
                    if (!company.AdvanceTo(currentDate))
                    {
                        //company data ended before currentDate
                        keepParsing = false;
                        break;
                    }
                }

                if (keepParsing)
                {
                    //See if all days are the same
                    var isSynced = true;
                    foreach (var company in companiesRaw)
                    {
                        if (!company.CurrentDate.Equals(currentDate))
                        {
                            isSynced = false;
                            break;
                        }
                    }

                    if (isSynced)
                    {
                        //all files are at the current date, add to history
                        var marketDay = new MarketDay(coverDate.ToString());
                        coverDate = NextWeekday(coverDate);
                        foreach (var company in companiesRaw)
                        {
                            var vStock = new ValuatedStock(company.CurrentDayData().Split(','), company.MetaData);

                            vStock.Close = (float)Math.Round((vStock.Close * mult), 2);
                            vStock.Open = (float)Math.Round((vStock.Open * mult), 2);
                            vStock.High = (float)Math.Round((vStock.High * mult), 2);
                            vStock.Low = (float)Math.Round((vStock.Low * mult), 2);
                            vStock.Volume = (int)(vStock.Volume / mult);

                            marketDay.TradedCompanies.Add(vStock);
                        }
                        _fullHistory.Add(marketDay);
                    }

                    companiesRaw[0].AdvanceDay();
                }
            }
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
