namespace Shared.Comms.Messages
{
    public class StockPriceUpdate : Message
    {
        public StockPriceUpdate()
        {

        }

        public Shared.EvaluatedStocks StocksList
        {
            get; set;
        }
    }
}
