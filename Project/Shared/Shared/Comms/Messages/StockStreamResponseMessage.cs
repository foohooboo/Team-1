using System.Collections.Generic;
using Shared;

namespace Shared.Comms.Messages
{
    public class StockStreamResponseMessage : Message
    {
        public EvaluatedStocks EvaluatedStocksList
        {
            get; set;
        }

        public StockStreamResponseMessage()
        {
            EvaluatedStocksList = new EvaluatedStocks();
        }
    }
}