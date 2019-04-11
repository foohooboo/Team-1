using log4net;
using Newtonsoft.Json;
using Shared.MarketStructures;
using Shared.PortfolioResources;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Broker
{
    public static class PortfolioManager
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string PortfolioData => $"";

        private static object LockPortfolios = new object();
        private static ConcurrentDictionary<int, Portfolio> _portfolios = new ConcurrentDictionary<int, Portfolio>();

        //Use this function to ALWAYS return a copy of the managed portfolio. You never want an outside source to have
        //access to the actual/live portfolios or they may inadvertently change them. The only time you want to access
        //a _portfolios entry directly is during the ProcessTransaction method, or maybe a password change. -Dsphar 4/11/2019
        public static Portfolio GetPortfolioSafe(int portfolioId)
        {
            Portfolio portfolio = null;
            lock (LockPortfolios)
            {
                if (!_portfolios.TryGetValue(portfolioId, out portfolio))
                    Log.Warn($"Could not find portfolio {portfolioId}.");
            }
            return portfolio;
        }

        public static bool TryToCreate(string username, string password, out Portfolio portfolio)
        {
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (enter)");

            bool success = false;

            portfolio = null;

            if (_portfolios.Values.Any(p => p.Username.Equals(username)))
            {
                Log.Warn($"Portfolio already exists with Username: {username}");
                return false;
            }

            lock (LockPortfolios)
            {
                var internalPortfolio = new Portfolio()
                {
                    PortfolioID = _portfolios.Count + 1,
                    Username = username,
                    Password = password,
                };

                internalPortfolio.Assets.Add("$", new Asset()
                {
                    RelatedStock = new Stock("$", "US Dollars"),
                    Quantity = 10000 // default cash value for new participants
                });

                if (_portfolios.TryAdd(internalPortfolio.PortfolioID, internalPortfolio))
                {
                    success = true;
                    portfolio = new Portfolio(internalPortfolio); //Never return the portfolio in _portfolios
                }
            }

            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (exit)");
            return success;
        }

        public static bool TryToGet(string username, string password, out Portfolio portfolio)
        {
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (enter)");

            bool sucess = false;
            portfolio = null;

            lock (LockPortfolios)
            {
                var localPorfolio = _portfolios.Where(
                    p => p.Value.Username.Equals(username) &&
                    p.Value.Password.Equals(password)).FirstOrDefault().Value ?? null;

                if (localPorfolio != null)
                {
                    sucess = true;
                    portfolio = new Portfolio(localPorfolio); //Never return the portfolio in _portfolios
                }
            }

            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (exit)");
            return sucess;
        }

        public static bool TryToGet(int portfolioID, out Portfolio portfolio)
        {
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (enter)");

            bool success = false;

            portfolio = GetPortfolioSafe(portfolioID);
            if (portfolio == null)
            {
                Log.Debug($"Failed to get portfolio for id: {portfolioID}");
            }
            else
            {
                success = true;
            }

            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (exit)");
            return success;
        }
        
        public static bool TryToRemove(int portfolioID)
        {
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (enter)");

            bool sucess = false;

            if (!_portfolios.ContainsKey(portfolioID))
            {
                Log.Warn($"No portfolio {portfolioID} to remove.");
            }
            else
            {
                lock (LockPortfolios)//probably not needed since we aren't modifying the localPortfolio, but added as reminder.
                {
                    sucess = _portfolios.TryRemove(portfolioID, out Portfolio localPortfolio);
                }
            }

            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (exit)");
            return sucess;
        }

        public static void SavePortfolios()
        {
            lock (LockPortfolios)
            {
                using (StreamWriter file = File.CreateText(PortfolioData))
                {
                    new JsonSerializer().Serialize(file, _portfolios.Values.ToArray());
                }
            }
        }

        public static void LoadPortfolios()
        {
            var loadedPortfolios = new List<Portfolio>();

            using (var reader = new StreamReader(PortfolioData))
            {
                loadedPortfolios = JsonConvert.DeserializeObject<List<Portfolio>>(reader.ReadToEnd());
            }

            lock (LockPortfolios) //probably not needed since we aren't modifying the localPortfolios, but added as reminder.
            {
                _portfolios.Clear();
                foreach (var portfolio in loadedPortfolios)
                {
                    _portfolios.TryAdd(portfolio.PortfolioID, portfolio);
                }
            }
        }


        /// <summary>
        /// Add(+qty) or remove(-qty) the desired quantity stock with the provided symbol.
        /// Do nothing and return false if entire transaction cannot be completed (insufficient funds, etc).
        /// </summary>
        /// <param name="portfolioId"></param>
        /// <param name="stockSymbol"></param>
        /// <param name="quantity"></param>
        /// <param name="updatedPortfolio"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool PerformTransaction(int portfolioId, string stockSymbol, float quantity, float price, out Portfolio updatedPortfolio, out string errorMessage)
        {
            bool success = false;
            errorMessage = "";
            updatedPortfolio = null;

            if (false)//TODO: check if price is valid for this stock
            {
                errorMessage = "Broker does not have record of the desired price. Transaction canceled.";
            }

            else if (quantity == 0)
            {
                errorMessage = "You cannot make a transaction for 0 stocks.";
            }

            else
            {
                //Lock is to fully complete transaction before opening up the internalPortfolio for others to access/change.
                //I suppose we could rely on the portfolio lock itself, but this logic below is fast enough it shouldn't matter.
                lock (LockPortfolios)
                {
                    if(_portfolios.TryGetValue(portfolioId, out Portfolio internalPortfolio))
                    {
                        Asset ownedAsset = internalPortfolio.Assets.Where(a => a.Value.RelatedStock.Symbol.Equals(stockSymbol)).LastOrDefault().Value;
                        Asset cashOnHand = internalPortfolio.Assets.Where(a => a.Value.RelatedStock.Symbol.Equals("$")).LastOrDefault().Value;

                        //Buying
                        if (quantity > 0)
                        {
                            var totalCost = quantity * price;
                            if(cashOnHand.Quantity < totalCost)
                            {
                                errorMessage = "You do not have enough cash to make this purchase. Transaction canceled.";
                            }
                            else
                            {
                                cashOnHand.Quantity -= totalCost;
                                if (ownedAsset == null)
                                {
                                    ownedAsset = new Asset(new Stock(stockSymbol, ""), quantity);
                                    internalPortfolio.Assets.Add(stockSymbol, ownedAsset);
                                }
                                else
                                {
                                    ownedAsset.Quantity += quantity;
                                }
                                updatedPortfolio = new Portfolio(internalPortfolio);//Copy so internal doesn't leave.
                                success = true;
                            }
                        }

                        //Selling (complete)
                        else
                        {
                            if (ownedAsset == null)
                            {
                                errorMessage = $"You do not own any {stockSymbol} stocks.";
                            }
                            else if (ownedAsset.Quantity < quantity)
                            {
                                errorMessage = $"You do not own {quantity} {stockSymbol} stocks. Transaction canceled";
                            }
                            else
                            {
                                ownedAsset.Quantity -= quantity;
                                cashOnHand.Quantity += price * quantity;
                                updatedPortfolio = new Portfolio(internalPortfolio);//Copy because internal should never leave this class.
                                success = true;
                            }
                        }
                    }
                    else
                    {
                        errorMessage = $"Could not find portfolio with id {portfolioId}. Transaction canceled.";
                    }
                }
            }
            return success;
        }

        public static int PortfolioCount
        {
            get
            {
                return _portfolios.Count;
            }
            private set { }
        }

        public static void Clear()
        {
            _portfolios.Clear();
        }

        public static Dictionary<int,Portfolio> Portfolios
        {
            get
            {
                var dictionary = new Dictionary<int, Portfolio>();

                lock (LockPortfolios)
                {
                    foreach (var entry in _portfolios)
                    {
                        dictionary.Add(entry.Key, new Portfolio(entry.Value));
                    }
                }
                return dictionary;
            }
            private set { }
        }
    }
}