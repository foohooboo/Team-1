namespace Shared.Comms.Messages
{
    public class StockHistoryRequestMessage : Message
    {
        public StockHistoryRequestMessage()
        {

        }

        public int TicksRequested { get; set; } = 75;
    }
}
