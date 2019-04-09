using Client.Conversations.StockHistory;
using Shared.Conversations;
using Shared.MarketStructures;
using Shared.PortfolioResources;
using System;
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

        public string SelectedStocksSymbol { get; set; } = "";

        public readonly Dictionary<string, List<ValuatedStock>> _stockHistoryBySymbol = new Dictionary<string, List<ValuatedStock>>();

        /// <summary>
        /// This static TraderModel is a shortcut to share the active TraderModel to anyone that needs it (like conversations).
        /// Can probably be done a "better" way, but this is good enough for our needs.  -Dsphar 4/8/2019
        /// </summary>
        public static TraderModel Current { get; set; }

        public TraderModel()
        {
            if (Current == null)
                Current = this;
            else
                throw new Exception("Only one Trader model should be used.");

            Current = this;
            var getStockHistConv = new StockHistoryRequestConversation();
            getStockHistConv.SetInitialState(new StockHistoryRequestState(getStockHistConv));
            ConversationManager.AddConversation(getStockHistConv);
        }

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

                Handler?.ReDrawPortfolioItems();
            }
        }

        private object stockHistoryLock = new object();
        public MarketSegment StockHistory
        {
            get
            {
                lock (stockHistoryLock)
                {

                    //Do we need to perform a deep copy of this so people using it don't mess with it??
                    return _stockHistory;
                }
            }
            set
            {
                lock (stockHistoryLock)
                {
                    _stockHistory = value;

                    foreach (var day in _stockHistory)
                    {
                        foreach (var vStock in day.TradedCompanies)
                        {
                            AddStockToHistory(vStock);
                        }
                    }

                    Handler?.StockHistoryChanged();
                    Handler?.ReDrawPortfolioItems();
                }
                
            }
        }

        private void AddStockToHistory(ValuatedStock vStock)
        {
            if (!_stockHistoryBySymbol.ContainsKey(vStock.Symbol))
            {
                _stockHistoryBySymbol.Add(vStock.Symbol, new List<ValuatedStock>());
            }

            //Question, do we want to limit the size of this history? Maybe by a configurable length?
            _stockHistoryBySymbol[vStock.Symbol].Add(vStock);

            //TODO: add candlestick for this day. Again, do we want to limit the history here?
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
                Handler?.ReDrawPortfolioItems();
            }
        }

        public SortedList<float, Asset> OwnedStocksByValue
        {
            get => _ownedStocksByValue;
            set
            {
                _ownedStocksByValue = value;
                Handler?.ReDrawPortfolioItems();
            }
        }
    }
}
