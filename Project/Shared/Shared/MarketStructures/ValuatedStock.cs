using System;

namespace Shared.MarketStructures
{
    [Serializable()]
    public class ValuatedStock : Stock
    {
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }
        public int Volume { get; set; }

        public ValuatedStock()
        {
            Open = 0.0F;
            High = 0.0F;
            Low = 0.0F;
            Close = 0.0F;
            Volume = 0;
        }

        public ValuatedStock(string[] data, Stock s) : base(s)
        {
            //Expected string[] format: date(unused),open,high,low,close,volume
            Open = float.Parse(data[1]);
            High = float.Parse(data[2]);
            Low = float.Parse(data[3]);
            Close = float.Parse(data[4]);
            Volume = int.Parse(data[5]);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ValuatedStock compareVStock))
                return false;

            return
                (
                Symbol == compareVStock.Symbol &&
                Name == compareVStock.Name &&
                Open == compareVStock.Open &&
                High == compareVStock.High &&
                Low == compareVStock.Low &&
                Close == compareVStock.Close &&
                Volume == compareVStock.Volume
                );
        }
    }
}
