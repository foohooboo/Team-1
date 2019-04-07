using log4net;
using Shared.MarketStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class Temp
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Temp()
        {

        }

        public void LogStockHistory(MarketSegment seg)
        {
            for (int i = 0; i < 5 && i < seg.Count; i++)
            {
                Log.Info($"Day {i}, {seg[i].TradedCompanies[1].Name}, Price:{seg[i].TradedCompanies[1].Close}");
            }
        }
    }
}
