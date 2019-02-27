using System.Collections.Generic;

namespace Shared.Comms.Messages
{
    class PortfolioUpdateMessage : Message
    {
        public PortfolioUpdateMessage()
        {
            assets = new Dictionary<string, Portfolio.Asset>();
        }

        public PortfolioUpdateMessage(Dictionary<string, Shared.Portfolio.Asset> Assets)
        {
            this.assets = Assets;
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

        public bool RequestWriteAuthority
        {
            get; set;
        }
    }
}
