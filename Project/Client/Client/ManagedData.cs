using Shared.MarketStructures;
using Shared.PortfolioResources;
using SharedResources.DataGeneration;
using System;
using System.Collections.Generic;

namespace Client
{
    internal class ManagedData
    {//Incredibly lazily made class, I will add a lot more functions here as i need to.
        public float Cash { get; set; } = 100000;
        public MarketSegment History { get; set; } = new MarketSegment();
        public Portfolio MyPortfolio { get; set; } = new Portfolio();
        public SortedList<string, string> HighScores { get; set; } = new SortedList<string, string>();

        public SortedList<Asset, float> observablePortfolio { get; set; } = new SortedList<Asset, float>();

        private static readonly Random random = new Random(54);
        private static float low, high, open, close, volume;
        //Bad data added in manaually for testing
        public static Portfolio makeupPortfolio(MarketDay input)
        {
            Portfolio ret = new Portfolio();
            foreach (ValuatedStock i in input.TradedCompanies)
            {
                int count = random.Next(0, 2);
                count *= random.Next(0, 100);
                ret.Assets.Add(i.Symbol, new Asset(i, count));
            }
            return ret;
        }

        public static MarketSegment makeupMarketSegment(int stocks, int days)
        {
            MarketSegment returns = new MarketSegment();
            List<Stock> madeupStocks = new List<Stock>(stocks);
            for (int i = 1; i <= stocks; i++)
            {
                madeupStocks.Add(makeupStock());
            }
            for (int i = 0; i < days; i++)
            {
                List<ValuatedStock> dayList = new List<ValuatedStock>(stocks);
                for (int j = 0; j < stocks; j++)
                {
                    dayList.Add(makeupValuatedStock(madeupStocks[j]));
                }
                MarketDay dayHolder = new MarketDay("3-26-2019", dayList.ToArray());
                returns.Add(dayHolder);
            }

            return returns;
        }
        private static Stock makeupStock()
        {
            return new Stock(DataGenerator.GetRandomString(4), DataGenerator.GetRandomString(15));
        }
        private static ValuatedStock makeupValuatedStock(Stock s)
        {
            var vStock = new ValuatedStock();

            vStock.Name = s.Name;
            vStock.Symbol = s.Symbol;

            vStock.Low = (float)random.Next(1000);
            vStock.High = (float)random.Next(100) + vStock.Low;
            vStock.Open = random.Next((int)vStock.Low, (int)vStock.High);
            vStock.Close = random.Next((int)vStock.Low, (int)vStock.High);
            vStock.Volume = random.Next(100000);

            return vStock;
        }

    }
}
