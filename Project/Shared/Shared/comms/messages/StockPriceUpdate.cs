using Shared.MarketStructures;

namespace Shared.Comms.Messages
{
    public class StockPriceUpdate : Message
    {
        public StockPriceUpdate()
        {

        }

        public MarketDay StocksList
        {
            get; set;
        }
    }
}
