namespace Shared.Comms.Messages
{
    public class GetPortfolioRequest : Message
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
