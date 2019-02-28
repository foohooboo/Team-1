using System;
using System.Collections.Generic;

namespace Shared
{

    //These classes are all basic skeletons. Please add funcitonality as you choose

    public class EvaluatedStocks: List<EvaluatedStock>//contains every evaluated stock for a day
    {//Reminder: inherits List and all of its methods.
        public String Date { get; set; }
        public EvaluatedStocks(string date)
        {
            Date = date;
        }
        public EvaluatedStocks(string date, EvaluatedStock[] starterArray)
        {
            Date = date;
            this.AddRange(starterArray);
        }
        public EvaluatedStocks() { }

    }

    public class Stock//no value, just holds the symbol and full name of company
    {
        public string Symbol{ get; set; }
        public string Name { get; set; }
        public Stock(string symbol, string name)
        {
            Symbol = symbol;
            Name = name;
        }
    }

    public class EvaluatedStock
    {
        public Stock stock { get; set; }
        
        public float open { get; set; }
        public float high { get; set; }
        public float low { get; set; }
        public float close { get; set; }
        public int volume { get; set; }
        public EvaluatedStock(string[] data, Stock s)
        {//date,open,high,low,close,volume  Date is unused here.
            open = float.Parse(data[1]);
            high = float.Parse(data[2]);
            low = float.Parse(data[3]);
            close = float.Parse(data[4]);
            volume = int.Parse(data[5]);
            stock =s;
        }
    }
}
