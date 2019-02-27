namespace Shared.Comms.Messages
{
    public class AckMessage : Message
    {
        public AckMessage()
        {

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
