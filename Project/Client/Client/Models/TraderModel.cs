using Client.Conversations.StockHistory;
using Shared;
using Shared.Conversations;
using Shared.Leaderboard;
using Shared.MarketStructures;
using Shared.PortfolioResources;
using SharedResources.Conversations.StockStreamRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using static Client.Conversations.StockUpdate.ReceiveStockUpdateState;

namespace Client.Models
{
    public class TraderModel
    {
        public IHandleTraderModelChanged Handler;

        public string SelectedStocksSymbol { get; set; } = "";
        private Portfolio _portfolio;
        private MarketSegment _stockHistory = new MarketSegment();
        private List<LeaderboardRecord> _leaderboard;

        private object LockOwnedStock = new object();
        private List<Asset> _ownedStocks = new List<Asset>();

        private Dictionary<string, List<ValuatedStock>> _stockHistoryBySymbol = new Dictionary<string, List<ValuatedStock>>();

        /// <summary>
        /// This static TraderModel is a shortcut to share the active TraderModel to anyone that needs it (like conversations).
        /// Sharing active TraderModel Can probably be done a "better" way, but this is sufficient for our needs.  -Dsphar 4/8/2019
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

            var stockStreamConv = new StockStreamRequestConversation(Config.GetClientProcessNumber());
            stockStreamConv.SetInitialState(new StockStreamRequestState(Config.GetClientProcessNumber(), stockStreamConv));
            ConversationManager.AddConversation(stockStreamConv);

            StockUpdateEventHandler += HandleStockUpdate;
        }

        public Portfolio Portfolio
        {
            get => _portfolio;
            set
            {
                _portfolio = value;
                lock (LockOwnedStock)
                {
                    _ownedStocks = value.Assets.Values.ToList();
                }
                QtyCash = value.Assets.Where(s => s.Key.Equals("$")).FirstOrDefault().Value.Quantity;
                Handler?.ReDrawPortfolioItems();
            }
        }

        public MarketSegment StockHistory
        {
            get
            {
                //return deep copy so user wont mess with current.
                var copy = new MarketSegment(_stockHistory);
                return copy;
            }
            set
            {
                _stockHistory = value;

                foreach (var day in _stockHistory)
                {
                    foreach (var vStock in day.TradedCompanies)
                    {
                        AddStockToHistory(vStock);
                    }
                }
                Handler?.ReDrawPortfolioItems();
            }
        }

        public List<ValuatedStock> GetHistory(string symbol)
        {
            _stockHistoryBySymbol.TryGetValue(symbol, out List<ValuatedStock> history);
            return history;
        }

        private void AddStockToHistory(ValuatedStock vStock)
        {
            if (!_stockHistoryBySymbol.ContainsKey(vStock.Symbol))
            {
                _stockHistoryBySymbol.Add(vStock.Symbol, new List<ValuatedStock>());
            }

            var history = _stockHistoryBySymbol[vStock.Symbol];
            while (history.Count > Config.GetInt(Config.MAX_STOCK_HISTORY))
            {
                history.RemoveAt(0);
            }

            history.Add(vStock);
        }

        public List<LeaderboardRecord> Leaderboard
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
            get
            {
                lock (LockOwnedStock)
                {
                    var cashAsAsset = _ownedStocks.Where(s => s.RelatedStock.Symbol.Equals("$")).FirstOrDefault();
                    if (cashAsAsset == null)
                        return 0;
                    else
                        return cashAsAsset.Quantity;
                }
            }
            private set { }
        }

        public List<Asset> OwnedStocksByValue
        {
            get
            {
                lock (LockOwnedStock)
                {
                    _ownedStocks.Sort((a, b) => -GetStockNet(a).CompareTo(GetStockNet(b)));
                    return _ownedStocks;
                }
            }
            set
            {
                lock (LockOwnedStock)
                {
                    _ownedStocks = value;
                }
                Handler?.ReDrawPortfolioItems();
            }
        }

        public List<Asset> OwnedStocksBySymbol
        {
            get
            {
                List<Asset> sortedBySymbol = new List<Asset>();
                lock (LockOwnedStock)
                {
                    sortedBySymbol = _ownedStocks.Select(asset => new Asset(asset.RelatedStock, asset.Quantity)).ToList();
                }
                sortedBySymbol.Sort((a, b) => a.RelatedStock.Symbol.CompareTo(b.RelatedStock.Symbol));

                return _ownedStocks;
            }
            set
            {
                lock (LockOwnedStock)
                {
                    _ownedStocks = value;
                }
                Handler?.ReDrawPortfolioItems();
            }
        }

        /// <summary>
        /// Returns the net worth of the given asset based on the most recent market value. If no recent value
        /// is known, assume $0.
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        public float GetStockNet(Asset stock)
        {
            return GetRecentValue(stock.RelatedStock.Symbol) * stock.Quantity;
        }

        /// <summary>
        /// Returns the latest known value of the given stock. 0 if no history is known for the provided symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public float GetRecentValue(string symbol)
        {
            float price = 0;
            List<ValuatedStock> hist;

            if (_stockHistoryBySymbol.TryGetValue(symbol, out hist) && hist.Count > 0)
            {
                price = hist.Last().Close;
            }

            return price;
        }

        public void HandleStockUpdate(object sender, StockUpdateEventArgs args)
        {
            foreach( var vStock in args.CurrentDay.TradedCompanies)
            {
                AddStockToHistory(vStock);
            }

            _stockHistory.Add(args.CurrentDay);
            while(_stockHistory.Count > Config.GetInt(Config.MAX_STOCK_HISTORY))
            {
                _stockHistory.RemoveAt(0);
            }

            Handler?.ReDrawPortfolioItems();
        }

        public void PassStatus(string message)
        {
            Handler?.ShowStatus(message);
        }
    }
}
