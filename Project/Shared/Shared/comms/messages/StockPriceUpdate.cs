using Shared;

namespace Shared.Comms.Messages
{
    public class StockPriceUpdate : Message
    {
        public StockPriceUpdate()
        {

        }

        public EvaluatedStocks StocksList
        {
            get; set;
        }
    }
}
