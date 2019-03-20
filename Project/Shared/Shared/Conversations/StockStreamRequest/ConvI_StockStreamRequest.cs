using Shared.Conversations.SharedStates;

namespace Shared.Conversations.StockStreamRequest.Initiator
{
    class ConvI_StockStreamRequest : Conversation
    {
        public readonly string StockServerAddress = $"{Config.GetString(Config.STOCK_SERVER_IP)}:{Config.GetInt(Config.STOCK_SERVER_PORT)}";

        public ConvI_StockStreamRequest(int processId):base(processId)
        {
        }
    }
}
