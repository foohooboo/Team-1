namespace Shared.Comms.Messages
{
    class CreatePortfolioRequestMessage : GetPortfolioRequest
    {
        public CreatePortfolioRequestMessage() : base()
        {

        }

        public string ConfirmPassword
        {
            get; set;
        }
    }
}
