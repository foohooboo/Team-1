using Broker;
using log4net;
using Shared.Client;
using Shared.Comms.MailService;
using Shared.Comms.Messages;

namespace Shared.Conversations.SharedStates
{
    public class LeaderboardSendUpdateState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Use this constructor if state is created locally and not from incoming message.
        public LeaderboardSendUpdateState(Conversation conv, ConversationState previousState) : base(conv, previousState) { }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            return null;
        }

        public override Envelope Prepare()
        {
            Log.Debug($"{nameof(Prepare)} (enter)");

            var message = MessageFactory.GetMessage<UpdateLeaderBoardMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), 0) as UpdateLeaderBoardMessage;

            foreach (var portfolio in PortfolioManager.Portfolios)
            {
                var record = LeaderboardManager.GetLeaderboardRecord(portfolio.Value);
                message.Records.Add(record.Username, record.TotalAssetValue);
            }
            
            ClientManager.UpdateClients(message);

            Log.Debug($"{nameof(Prepare)} (exit)");
            return null;
        }
    }
}