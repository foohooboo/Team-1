namespace Shared.Comms.Messages
{
    public class AckMessage : Message
    {
        public AckMessage(int ReferenceMessageID)
        {
            this.ReferenceMessageID = ReferenceMessageID;
        }

        public int ReferenceMessageID
        {
            get; set;
        }

        public string AckHello
        {
            get; set;
        }
    }
}
