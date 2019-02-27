using System.Collections.Generic;

namespace Shared.Comms.Messages
{
    public class StockStreamResponseMessage : Message
    {
        //TODO: Update this list type
        public List<object> EvaluatedStocks
        {
            get; set;
        }

        public StockStreamResponseMessage()
        {

        }

        //TODO: Add way to set EvaluatedStocks list data.
    }
}