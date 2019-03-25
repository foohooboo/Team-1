using System.Collections.Concurrent;
using System.Linq;
using log4net;
using Shared.MarketStructures;
using Shared.Portfolio;

namespace Broker
{
    public static class PortfolioManager
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ConcurrentDictionary<int, Portfolio> Portfolios = new ConcurrentDictionary<int, Portfolio>();

        public static bool TryToCreate(string username, string password, out Portfolio portfolio)
        {
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (enter)");
            portfolio = null;

            if (Portfolios.Values.Any(p => p.Username.Equals(username)))
            {
                Log.Warn($"Already a portfolio with Username: {username}");
                return false;
            }

            var newPortfolio = GetNewPortfolio(username, password);

            if (!Portfolios.TryAdd(newPortfolio.PortfolioID, newPortfolio))
            {
                Log.Debug($"Failed to add portfolio for {username}");
                return false;
            }

            portfolio = newPortfolio;
            portfolio.WriteAuthority = true;

            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (exit)");
            return true;
        }

        private static Portfolio GetNewPortfolio(string username, string password)
        {
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (enter)");
            var portfolio = new Portfolio()
            {
                PortfolioID = Portfolios.Count + 1,
                Username = username,
                Password = password
            };

            var cash = new Asset()
            {
                RelatedStock = new Stock("$", "US Dollars"),
                Quantity = 10000 // default cash value for participants
            };

            portfolio.ModifyAsset(cash);

            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (exit)");
            return portfolio;
        }

        public static bool TryToGet(int portfolioID, out Portfolio portfolio)
        {
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (enter)");

            if (!Portfolios.TryGetValue(portfolioID, out portfolio))
            {
                Log.Debug($"Failed to get portfolio for id: {portfolioID}");
                return false;
            }

            if (portfolio.WriteAuthority)
            {
                Log.Debug($"Portfolio for {portfolio.Username} is currently locked.");
                portfolio = null;
                return false;
            }

            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (exit)");
            portfolio.WriteAuthority = true;
            return true;
        }

        public static bool TryToRemove(int portfolioID)
        {
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (enter)");

            if (!Portfolios.TryGetValue(portfolioID, out Portfolio portfolio))
            {
                Log.Debug($"Failed to get portfolio for id: {portfolioID}");
                return false;
            }

            if (portfolio.WriteAuthority)
            {
                Log.Debug($"Portfolio for {portfolio.Username} is currently locked.");
                return false;
            }

            if (!Portfolios.TryRemove(portfolio.PortfolioID, out Portfolio removedPortfolio))
            {
                Log.Debug($"Failed to remove portfolio for id: {portfolio.PortfolioID}");
                return false;
            }

            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (exit)");
            return true;
        }

        public static void ReleaseLock(Portfolio portfolio)
        {
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (enter)");
            portfolio.WriteAuthority = false;
            portfolio = null;
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (exit)");
        }
    }
}