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

        public Shared.EvaluatedStock StockValue
        {
            get; set;
        }
    }
}
