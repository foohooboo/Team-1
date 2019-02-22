using System.Collections.Generic;

namespace Shared.Comms.Messages
{
    public class StockStreamResponseMessage : Message
    {
        public List<object> EvaluatedStocks
        {
            get; set;
        }

        public StockStreamResponseMessage()
        {

        }
    }
}