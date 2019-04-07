namespace Shared.Comms.Messages
{
    public class ErrorMessage : AckMessage
    {
        public ErrorMessage()
        {

        }

        public string ErrorText
        {
            get; set;
        }
    }
}
