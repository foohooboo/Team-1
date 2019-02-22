namespace Shared.Comms.Messages
{
    public class StockStreamRequestMessage : Message
    {
        public int TicksRequested
        {
            get => 30;
        }

        public StockStreamRequestMessage()
        {

        }
    }
}
