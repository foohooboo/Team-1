using System.Collections.Generic;

namespace Shared.Comms.Messages
{
    public class PortfolioUpdateMessage : Message
    {
        public PortfolioUpdateMessage()
        {
            Assets = new Dictionary<string, Portfolio.Asset>();
        }

        public PortfolioUpdateMessage(Dictionary<string, Shared.Portfolio.Asset> Assets)
        {
            this.Assets = Assets;
        }

        public int PortfolioID
        {
            get; set;
        }

        public Dictionary<string, Shared.Portfolio.Asset> Assets
        {
            get; set;
        }

        /*
         * cash is represented as an asset in the Portfolio.Assets dictionary as with symbol "CASH" and name "CASH"
        public double CashValue
        {
            get; set;
        }
        */

        public bool WriteAuthority
        {
            get; set;
        }
    }
}
