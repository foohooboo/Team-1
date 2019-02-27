namespace Shared.Comms.Messages
{
    class CreatePortfolioRequestMessage : GetPortfolioRequest
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
