using Shared.MarketStructures;
using Shared.PortfolioResources;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Client
{
    public class TraderModel
    {

        public IHandleTraderModelChanged Handler;

        private Portfolio _portfolio;
        private MarketSegment _stockHistory;
        public SortedList<float,string> _leaderboard;

        /// <summary>
        /// This static TraderModel is a shortcut to share the active TraderModel to anyone that needs it (like conversations).
        /// Can probably be done a "better" way, but this is good enough for our needs.  -Dsphar 4/8/2019
        /// </summary>
        public static TraderModel Current { get; set; }

        public Portfolio Portfolio
        {
            get => _portfolio;
            set
            {
                _portfolio = value;
                Handler?.ProfileChanged();
            }
        }

        public MarketSegment StockHistory
        {
            get => _stockHistory;
            set
            {
                _stockHistory = value;
                Handler?.StockHistoryChanged();
            }
        }

        public SortedList<float,string> Leaderboard{
            get => _leaderboard;
            set
            {
                _leaderboard = value;
                Handler?.LeaderboardChanged();
            }
        }
    }
}
