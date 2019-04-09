using Shared.MarketStructures;
using Shared.PortfolioResources;
using SharedResources.DataGeneration;
using System;
using System.Collections.Generic;
using System.Linq;

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
            for (int i = 0; i < stocks; i++)
            {
                madeupStocks.Add(makeupStock());
            }
            for (int i = 0; i < days; i++)
            {
                List<ValuatedStock> dayList = new List<ValuatedStock>(stocks);
                
                for (int j = 0; j < madeupStocks.Count; j++)
                {
                    var symbol = madeupStocks[j].Symbol;
                    if (symbol.Equals("$"))
                        continue;

                    float previousClose;
                    if (i > 0)
                        previousClose = returns[i - 1].TradedCompanies.Where(s => s.Symbol.Equals(symbol)).FirstOrDefault().Close;
                    else
                        previousClose = random.Next(100, 500);

                    var vStock = MakeupValuatedStock(madeupStocks[j].Symbol, madeupStocks[j].Name, previousClose);
                    dayList.Add( vStock );
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

        private static ValuatedStock makeupValuatedStock(Stock stock, float previousDayClose)
        {
            return MakeupValuatedStock(stock.Symbol, stock.Name, previousDayClose);
        }

        private static ValuatedStock MakeupValuatedStock(string symbol, string name, float previousDayClose)
        {
            var vStock = new ValuatedStock();

            vStock.Name = name;
            vStock.Symbol = symbol;

            //Make trading look realistic relative to previous day
            var scaler = (int) (previousDayClose * 0.01);

            vStock.Open =  previousDayClose + random.Next(-scaler/2, scaler); 
            vStock.Close = vStock.Open + random.Next(-scaler*3, scaler*3);

            vStock.Low = Math.Min(vStock.Open, vStock.Close) - random.Next(0, scaler * 3);
            vStock.High = Math.Max(vStock.Open, vStock.Close) + random.Next(0, scaler * 3);

            vStock.Volume = (int)Math.Abs(vStock.Open - vStock.Close)*1000000;

            return vStock;
        }

    }
}
