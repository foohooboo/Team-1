namespace Shared.Comms.Messages
{
    public class AckMessage : Message
    {
        public AckMessage()
        {

        }

        public string ReferenceMessageID
        {
            get; set;
        }

        public string AckHello
        {
            get; set;
        }
    }
}
