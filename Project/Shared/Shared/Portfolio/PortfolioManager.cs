using log4net;
using Shared.MarketStructures;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Shared.Portfolio
{
    /// <summary>
    /// This class was thrown together so I could manage a couple Transaction Request Conversation tests.
    /// We should clean it up and add-on as necessary.
    /// </summary>
    public class PortfolioManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static ConcurrentDictionary<string, Portfolio> Portfolios = new ConcurrentDictionary<string, Portfolio>();

        private static int Counter;

        public static Portfolio CreatePortfolio(string username, string password)
        {
            Log.Debug($"{nameof(CreatePortfolio)} (enter)");

            Portfolio portfolio = null;

            if (Portfolios.ContainsKey(username))
            {
                Log.Warn($"Already a portfolio with Username: {username}");
            }
            else
            {
                var port = new Portfolio()
                {
                    PortfolioID = ++Counter,
                    Username = username,
                    Password = password
                };

                if (Portfolios.TryAdd(username, port))
                {
                    portfolio = port;
                    var cash = new Asset()
                    {
                        RelatedStock = new Stock("$", "US Dollars"),
                        Quantity = 10000
                    };
                    portfolio.ModifyAsset(cash);
                }                
            }

            Log.Debug($"{nameof(CreatePortfolio)} (exit)");
            return portfolio;
        }

        public static Portfolio GetPortfolio(int id)
        {
            Log.Debug($"{nameof(GetPortfolio)} (enter)");

            Portfolio port = null;
            foreach(var portfolio in Portfolios.Values)
            {
                if (portfolio.PortfolioID == id)
                {
                    port = portfolio;
                    break;
                }
            }

            if (port == null)
                Log.Warn($"Could not portfolio with Id {id}.");

            Log.Debug($"{nameof(GetPortfolio)} (exit)");
            return port;
        }
    }
}
