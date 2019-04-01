using System;
using log4net;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.MarketStructures;

namespace Client.Conversations.StockUpdate
{
    public class ReceiveStockUpdateState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MarketDay StockUpdate
        {
            get; private set;
        }

        public static event EventHandler<StockUpdateEventArgs> StockUpdateEventHandler;

        public class StockUpdateEventArgs : EventArgs
        {
            public MarketDay CurrentDay
            {
                get; private set;
            }

            public StockUpdateEventArgs(MarketDay day)
            {
                CurrentDay = day;
            }
        }

        public ReceiveStockUpdateState(Envelope env, Conversation conversation, ConversationState parentState) : base(conversation, parentState)
        {

        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            Log.Debug($"{nameof(HandleMessage)} (enter)");

            ConversationState state = null;

            if (incomingMessage.Contents is StockPriceUpdate m)
            {
                state = new ConversationDoneState(Conversation, this);
                StockUpdate = m.StocksList;
            }

            Log.Debug($"{nameof(HandleMessage)} (exit)");
            return state;
        }

        public override Envelope Prepare()
        {
            StockUpdateEventHandler?.Invoke(this, new StockUpdateEventArgs(StockUpdate));
            return null;
        }
    }
}