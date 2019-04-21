namespace Shared.Comms.Messages
{
    public class StockStreamRequestMessage : Message
    {
        public StockStreamRequestMessage()
        {

        }

        public int TicksRequested { get; set; } = 75;
    }
}
