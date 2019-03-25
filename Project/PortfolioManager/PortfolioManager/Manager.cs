using System;
using System.Collections.Concurrent;
using System.Linq;
using log4net;
using Shared.MarketStructures;
using Shared.Portfolio;

namespace PortfolioManager
{
    public class Manager
    {
        private readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected readonly ConcurrentDictionary<int, Portfolio> portfolios;

        public Manager()
        {
            portfolios = new ConcurrentDictionary<int, Portfolio>();
        }

        public bool TryToCreatePortfolio(string username, string password, out Portfolio portfolio)
        {
            Log.Debug($"{nameof(TryToCreatePortfolio)} (enter)");
            portfolio = null;

            if (portfolios.Values.Any(p => p.Username.Equals(username)))
            {
                Log.Warn($"Already a portfolio with Username: {username}");
                return false;
            }

            var newPortfolio = GetNewPortfolio(username, password);

            if (!portfolios.TryAdd(newPortfolio.PortfolioID, newPortfolio))
            {
                Log.Debug($"Failed to add portfolio for {username}");
            }

            Log.Debug($"{nameof(TryToCreatePortfolio)} (exit)");
            return true;
        }

        protected Portfolio GetNewPortfolio(string username, string password)
        {
            Log.Debug($"{nameof(GetNewPortfolio)} (enter)");
            var portfolio = new Portfolio()
            {
                PortfolioID = portfolios.Count + 1,
                Username = username,
                Password = password
            };

            var cash = new Asset()
            {
                RelatedStock = new Stock("$", "US Dollars"),
                Quantity = 10000 // default cash value for participants
            };

            portfolio.ModifyAsset(cash);

            Log.Debug($"{nameof(GetNewPortfolio)} (exit)");
            return portfolio;
        }

        protected bool TryToAdd(Portfolio portfolio)
        {
            Log.Debug($"{nameof(TryToAdd)} (enter)");
            try
            {
                portfolios.TryAdd(portfolio.PortfolioID, portfolio);
            }
            catch (OverflowException)
            {
                Log.Debug($"Portfolios are at max capacity");
                return false;
            }

            Log.Debug($"{nameof(TryToAdd)} (exit)");
            return true;
        }

        public bool TryToUpdate(Portfolio portfolio)
        {
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (enter)");
            try
            {
                if (!portfolios.TryGetValue(portfolio.PortfolioID, out Portfolio storedPortfolio))
                {
                    Log.Debug($"The portfolio does not exist");
                    return false;
                }

                // Unlock the portfolio on update.
                if (portfolio.RequestWriteAuthority)
                {
                    portfolio.RequestWriteAuthority = false;
                }

                portfolios.TryUpdate(portfolio.PortfolioID, portfolio, storedPortfolio);
                Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (exit)");
                return true;
            }
            catch (OverflowException)
            {
                Log.Debug($"Portfolios are at max capacity");
                return false;
            }
        }

        public bool TryToRemove(Portfolio portfolio, out string errorMessage)
        {
            errorMessage = String.Empty;

            try
            {
                portfolios.TryRemove(portfolio.PortfolioID, out Portfolio removedPortfolio);
            }
            catch (OverflowException)
            {
                errorMessage = "Portfolios are at max capacity";
                return false;
            }

            return true;
        }

        public bool TryToGetWithLock(int portfolioID, out Portfolio portfolio)
        {
            if (!TryToGet(portfolioID, out portfolio))
            {
                return false;
            }

            portfolio.RequestWriteAuthority = true;

            return TryToUpdate(portfolio);
        }

        public bool TryToGet(int portfolioID, out Portfolio portfolio)
        {

            if (!portfolios.TryGetValue(portfolioID, out portfolio))
            {
                return false;
            }

            //
            if (portfolio.RequestWriteAuthority)
            {
                return false;
            }

            return true;
        }
    }
}