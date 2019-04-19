using Shared.Leaderboard;
using Shared.MarketStructures;
using Shared.PortfolioResources;
using System.Linq;

namespace Broker
{
    public static class LeaderboardManager
    {
        public static MarketDay Market
        {
            get; set;
        }

        public static LeaderboardRecord GetLeaderboardRecord(Portfolio portfolio)
        {
            float totalAssetValue = 0;

            foreach (var asset in portfolio.Assets)
            {
                var stockValue = Market.TradedCompanies.Where(s => s.Symbol == asset.Key).FirstOrDefault()?.Close ?? 0;
                float assetValue = stockValue * asset.Value.Quantity;
                totalAssetValue += assetValue;
            }

            return new LeaderboardRecord(portfolio.Username, totalAssetValue);
        }
    }
}
