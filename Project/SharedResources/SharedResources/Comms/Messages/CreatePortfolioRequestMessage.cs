namespace Shared.Comms.Messages
{
    public class CreatePortfolioRequestMessage : GetPortfolioRequest
    {
        public CreatePortfolioRequestMessage()
        {

        }

        public string ConfirmPassword
        {
            get; set;
        }
    }
}
