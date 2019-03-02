using Shared.MarketStructures;

namespace Shared.Comms.Messages
{
    public class StockPriceUpdate : Message
    {
        public StockPriceUpdate()
        {

        }

        public StockMarketDay StocksList
        {
            get; set;
        }
    }
}
