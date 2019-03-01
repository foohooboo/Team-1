using Shared;

namespace Shared.Comms.Messages
{
    public class TransactionRequestMessage : Message
    {
        public TransactionRequestMessage()
        {

        }

        //Positive value for buy, negative value for sell
        public int Quantity
        {
            get; set;
        }

        public EvaluatedStock StockValue
        {
            get; set;
        }
    }
}
