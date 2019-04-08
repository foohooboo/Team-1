using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.MarketStructures;
using Shared.PortfolioResources;

namespace Client
{
    public class TraderModel
    {

        public static TraderModel Current { get; set; }

        public Portfolio Portfolio { get; set; }

        public MarketSegment StockHistory { get; set; }

        public SortedList Leaderboard { get; set; }


    }
}
