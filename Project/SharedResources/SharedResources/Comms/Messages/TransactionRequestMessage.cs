using Shared.MarketStructures;

namespace Shared.Comms.Messages
{
    public class TransactionRequestMessage : Message
    {
        public float Quantity { get; set; }//Positive value for buy, negative value for sell
        public ValuatedStock StockValue { get; set; }
        public int PortfolioId { get; set; }

        public TransactionRequestMessage()
        {
            StockValue = new ValuatedStock();
        }

        public TransactionRequestMessage(TransactionRequestMessage message)
        {
            Quantity = message.Quantity;
            StockValue = message.StockValue;
        }

        public TransactionRequestMessage(float quantity, ValuatedStock stockValue)
        {
            Quantity = quantity;
            StockValue = stockValue;
        }
    }
}
