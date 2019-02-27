namespace Shared.Comms.Messages
{
    class GetPortfolioRequest : Message
    {
        public GetPortfolioRequest() : base()
        {

        }

        public Shared.Portfolio.Portfolio Account
        {
            get; set;
        }
    }
}
