using Broker;
using log4net;
using Shared.Client;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using System.Net;

namespace Shared.Conversations.SharedStates
{
    public class LeaderboardSendUpdateState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Use this constructor if state is created locally and not from incoming message.
        public LeaderboardSendUpdateState(IPEndPoint recipiant, Conversation conv, ConversationState previousState) : base(conv, previousState)
        {
            To = recipiant;
        }

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
                message.Records.Add(record.TotalAssetValue, record.Username);
            }

            var env = new Envelope(message) {
                To = this.To
            };

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }

        public override void HandleTimeout()
        {
            if (++CountRetrys <= Config.GetInt(Config.DEFAULT_RETRY_COUNT))
            {
                Log.Warn($"Initiating retry for conversation {Conversation.Id}.");
                Send();
            }
            else
            {
                Log.Info($"Client {OutboundMessage.To.ToString()} appears to be disconnected. Removing from connected clients.");
                ClientManager.TryToRemove(OutboundMessage.To);
                ConversationManager.GetConversation(Conversation.Id).UpdateState(new ConversationDoneState(Conversation, this));
            }

           
        }
    }
}