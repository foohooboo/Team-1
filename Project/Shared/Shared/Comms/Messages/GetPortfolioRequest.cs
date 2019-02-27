namespace Shared.Comms.Messages
{
    class GetPortfolioRequest : Message
    {
        public GetPortfolioRequest()
        {

        }

        public Shared.Portfolio.Portfolio Account
        {
            get; set;
        }
    }
}
