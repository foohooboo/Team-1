using System;
using System.Collections.Concurrent;
using Shared.Portfolio;

namespace PortfolioManager
{
    public class Manager
    {
        protected readonly ConcurrentDictionary<int, Portfolio> portfolios;

        public Manager()
        {
            portfolios = new ConcurrentDictionary<int, Portfolio>();
        }

        public bool TryToAdd(Portfolio portfolio, out string errorMessage)
        {
            errorMessage = String.Empty;

            try
            {
                portfolios.TryAdd(portfolio.PortfolioID, portfolio);
            }
            catch (OverflowException)
            {
                errorMessage = "Portfolios are at max capacity";
                return false;
            }

            return true;
        }

        public bool TryToUpdate(Portfolio portfolio, out string errorMessage)
        {
            errorMessage = String.Empty;

            try
            {
                if (!portfolios.TryGetValue(portfolio.PortfolioID, out Portfolio storedPortfolio))
                {
                    errorMessage = "The portfolio does not exist";
                    return false;
                }

                // Unlock the portfolio on update.
                if (portfolio.RequestWriteAuthority)
                {
                    portfolio.RequestWriteAuthority = false;
                }

                portfolios.TryUpdate(portfolio.PortfolioID, portfolio, storedPortfolio);
                return true;
            }
            catch (OverflowException)
            {
                errorMessage = "Portfolios are at max capacity";
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

            if (TryToUpdate(portfolio, out string error))
            {
                return false;
            }

            return true;
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