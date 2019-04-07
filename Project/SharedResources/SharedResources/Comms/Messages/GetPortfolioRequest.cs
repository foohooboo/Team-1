namespace Shared.Comms.Messages
{
    public class GetPortfolioRequest : Message
    {
        public GetPortfolioRequest()
        {

        }

        public Shared.PortfolioResources.Portfolio Account
        {
            get; set;
        }
    }
}
