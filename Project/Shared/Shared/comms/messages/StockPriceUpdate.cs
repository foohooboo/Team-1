using Shared.MarketStructures;

namespace Shared.Comms.Messages
{
    public class StockPriceUpdate : Message
    {
        public StockPriceUpdate()
        {
            StocksList = new MarketDay();
        }

        public StockPriceUpdate(MarketDay marketDay)
        {
            StocksList = marketDay;
        }

        public MarketDay StocksList
        {
            get; set;
        }
    }
}
