using Shared.MarketStructures;
using Shared.PortfolioResources;
using System.Collections;

namespace Client
{
    public class TraderModel
    {
        /// <summary>
        /// Shortcut to share the active TraderModel to anyone that needs it (like conversations).
        /// Can probably be done a "better" way, but this is good enough for our needs. 
        /// -Dsphar 4/8/2019
        /// </summary>
        public static TraderModel Current { get; set; }

        public Portfolio Portfolio { get; set; }

        public MarketSegment StockHistory { get; set; }

        public SortedList Leaderboard { get; set; }

    }
}
