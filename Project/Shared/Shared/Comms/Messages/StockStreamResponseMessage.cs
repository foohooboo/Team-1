using System.Collections.Generic;
using Shared;

namespace Shared.Comms.Messages
{
    public class StockStreamResponseMessage : Message
    {
        public List<EvaluatedStocks> EvaluatedStocksList
        {
            get; set;
        }

        public StockStreamResponseMessage()
        {

        }

        //TODO: Add way to set EvaluatedStocks list data.
    }
}