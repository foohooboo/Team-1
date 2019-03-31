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
            Log.Debug($"{nameof(HandleMessage)} (enter)");

            ConversationState nextState = null;

            switch (incomingMessage.Contents)
            {
                case AckMessage m:
                    nextState = new ConversationDoneState(Conversation, this);
                    break;
            }

            Log.Debug($"{nameof(HandleMessage)} (exit)");
            return nextState;
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

            var env = new Envelope(message, Config.GetString(Config.BROKER_IP), Config.GetInt(Config.BROKER_PORT))
            {
                To = this.To
            };

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }

        public override void HandleTimeout()
        {
            base.HandleTimeout();

            if (CountRetrys > Config.GetInt(Config.DEFAULT_RETRY_COUNT))
            {
                Log.Info($"Client {OutboundMessage.To.ToString()} appears to be disconnected. Removing from connected clients.");
                ClientManager.TryToRemove(OutboundMessage.To);
            }
        }
    }
}