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
        public Portfolio myPortfolio { get; set; } = new Shared.PortfolioResources.Portfolio();
        public SortedList<string, string> HighScores { get; set; } = new SortedList<string, string>();
        public Stock SelectedStock { get; set; } = new Stock("AAPL", "Apple Inc.");

        private static readonly Random random = new Random();
        private float low, high, open, close, volume;
        //Bad data added in manaually for testing
        public void makeUpData()
        {

            for (int i = 0; i < 15; i++)
            {

            }
            Stock apl = new Stock("AAPL", "Apple Inc.");
            //Expected string[] format: date(unused),open,high,low,close,volume
            string[] dat = { "3-27-2019", "21.34", "23.45", "21.10", "23.10", "4007" };
            ValuatedStock[] stocks = { new ValuatedStock(dat, apl) };
            MarketDay a = new MarketDay("3-27-2019", stocks);
            History.Add(a);

            Asset aplasset = new Asset(apl, 47);

        }
        private Stock makeupStock()
        {

            return new Stock(random.Next().ToString().GetHashCode().ToString("X"), random.Next().ToString().GetHashCode().ToString("x"));

        }
        private ValuatedStock makeupValuatedStock(Stock s)
        {
            low = random.Next() * 100;
            high = random.Next() * 10 + low;
            open = random.Next() * (high - low) + low;
            close = random.Next() * (high - low) + low;
            volume = (float)Math.Floor((double)random.Next() * 10000);
            string[] holder = { "3-29-19", low.ToString("0.00"), high.ToString("0.00"), open.ToString("0.00"), close.ToString("0.00"), volume.ToString("0") };
            return new ValuatedStock(holder, s);
        }

    }
}
