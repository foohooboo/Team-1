namespace Shared.Comms.Messages
{
    public class ErrorMessage : AckMessage
    {
        public ErrorMessage(int ReferenceMessageID) : base(ReferenceMessageID)
        {

        }

        public string ErrorText
        {
            get; set;
        }
    }
}
