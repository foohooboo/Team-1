using Shared.MarketStructures;
using Shared.Security;
using System;

namespace Shared.Comms.Messages
{
    public class StockPriceUpdate : Message
    {
        SignatureService sigServer = new SignatureService();

        public StockPriceUpdate()
        {
            SerializedStockList = Convert.ToBase64String(sigServer.Serialize(new MarketDay()));
        }

        public StockPriceUpdate(MarketDay marketDay)
        {
            SerializedStockList = Convert.ToBase64String(sigServer.Serialize(marketDay));
        }

        /// <summary>
        /// Note: we had to use a serialized stock list here instead of a MarketDay StockList because
        /// the json encode/decode process was breaking signature validation. providing the
        /// json serializer with an already serialized (as string) stock list fixed the problem.
        /// -Dsphar 4/6/2019
        /// </summary>
        public string SerializedStockList { get; set; }

        public string StockListSignature { get; set; }

        public MarketDay StocksList
        {
            get { return sigServer.Deserialize<MarketDay>(Convert.FromBase64String(SerializedStockList)); }
            private set { }
        }
    }
}
