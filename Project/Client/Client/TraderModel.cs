using Shared.MarketStructures;
using Shared.PortfolioResources;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Client
{
    public class TraderModel
    {
        public IHandleTraderModelChanged Handler;

        private Portfolio _portfolio;
        private MarketSegment _stockHistory;
        private SortedList<float,string> _leaderboard;
        private float _qtyCash;
        private SortedList<float, Asset> _ownedStocksByValue = new SortedList<float, Asset>();


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

                _ownedStocksByValue.Clear();
                foreach(var ownedStock in value.Assets.Values)
                {
                    //TODO: Either changed the sortedlist to OwnedByQty, or factor in prices right here.
                    _ownedStocksByValue.Add(ownedStock.Quantity, ownedStock);
                }

                QtyCash = value.Assets.Where(s => s.Key.Equals("$")).FirstOrDefault().Value.Quantity;

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

        public SortedList<float,string> Leaderboard
        {
            get => _leaderboard;
            set
            {
                _leaderboard = value;
                Handler?.LeaderboardChanged();
            }
        }

        public float QtyCash
        {
            get => _qtyCash;
            set
            {
                _qtyCash = value;
                Handler?.ReDrawPortfolio();
            }
        }

        public SortedList<float, Asset> OwnedStocksByValue
        {
            get => _ownedStocksByValue;
            set
            {
                _ownedStocksByValue = value;
                Handler?.ReDrawPortfolio();
            }
        }
    }
}
