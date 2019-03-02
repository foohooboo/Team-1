using Shared.MarketStructures;

namespace Shared.Comms.Messages
{
    public class TransactionRequestMessage : Message
    {
        public TransactionRequestMessage()
        {
            StockValue = new ValuatedStock();
        }

        public TransactionRequestMessage(TransactionRequestMessage message)
        {
            Quantity = message.Quantity;
            StockValue = message.StockValue;
        }

        public TransactionRequestMessage(int quantity, ValuatedStock stockValue)
        {
            Quantity = quantity;
            StockValue = stockValue;
        }

        //Positive value for buy, negative value for sell
        public int Quantity
        {
            get; set;
        }

        public ValuatedStock StockValue
        {
            get; set;
        }
    }
}
