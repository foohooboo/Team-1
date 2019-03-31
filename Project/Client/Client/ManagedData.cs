using System;
using System.Collections.Generic;
using Shared.MarketStructures;
using Shared.PortfolioResources;

namespace Client
{
    internal class ManagedData
    {//Incredibly lazily made class, I will add a lot more functions here as i need to.
        public float Cash { get; set; } = 100000;
        public MarketSegment History { get; set; } = new MarketSegment();
        public Portfolio MyPortfolio { get; set; } = new Portfolio();
        public SortedList<string, string> HighScores { get; set; } = new SortedList<string, string>();
        public Asset SelectedAsset{ get; set; } = new Asset(new Stock("APPL","Apple Inc."), 5);

        private static readonly Random random = new Random(54);
        private static float low, high, open, close,volume;
        //Bad data added in manaually for testing
        public static Portfolio makeupPortfolio(MarketDay input)
        {
            
            Portfolio ret = new Portfolio();
            foreach(ValuatedStock i in input.TradedCompanies)
            {
                int count=random.Next(0,1);
                count *= random.Next(0, 100);
                ret.Assets.Add(i.Symbol,new Asset(i,count));
            }
            return ret;
        }

        public static MarketSegment makeupMarketSegment(int stocks, int days)
        {
            MarketSegment returns = new MarketSegment();
            List<Stock> madeupStocks = new List<Stock>(stocks);
            for (int i = 0; i < stocks; i++)
            {
                madeupStocks.Add(makeupStock());
            }
            for(int i = 0; i < days; i++)
            {
                List<ValuatedStock> dayList = new List<ValuatedStock>(stocks);
                for(int j = 0; j < stocks; j++)
                {
                    dayList.Add(makeupValuatedStock(madeupStocks[j]));
                }
                MarketDay dayHolder = new MarketDay("3-26-2019",dayList.ToArray());
                returns.Add(dayHolder);
            }

            return returns;
        }
        private static Stock makeupStock()
        {
            string full = random.Next().ToString().GetHashCode().ToString("X");
            return new Stock(System.Text.RegularExpressions.Regex.Replace(full, @"[\d-]", string.Empty),full);

        }
        private static ValuatedStock makeupValuatedStock(Stock s)
        {
            low = (float)random.Next(10000)/100;
            high = (float)random.Next(1000)/100 + low;
            open = (float)random.Next((int)low * 100, (int)high * 100) / 100;
            close = (float)random.Next((int)low * 100, (int)high * 100) / 100;
            volume = random.Next(100000);
            string[] holder = { "3-29-19",low.ToString("0.00"), high.ToString("0.00"), open.ToString("0.00"), close.ToString("0.00"), volume.ToString("0") };
            return new ValuatedStock(holder, s);

        }

    }
}
