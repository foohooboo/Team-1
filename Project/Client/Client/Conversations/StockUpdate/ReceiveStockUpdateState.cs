using System;
using log4net;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.MarketStructures;
using Shared.Security;

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

        public ReceiveStockUpdateState(Envelope env, Conversation conversation) : base(conversation, null)
        {
            var update = env.Contents as StockPriceUpdate;
            var sigServ = new SignatureService();
            var bytes = Convert.FromBase64String(update.SerializedStockList);
            StockUpdate = sigServ.Deserialize<MarketDay>(bytes);
        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            Log.Debug($"{nameof(HandleMessage)} (enter)");

            //This state doesn't expect any incoming messages

            Log.Debug($"{nameof(HandleMessage)} (exit)");
            return null;
        }

        public override void Send()
        {
            base.Send();
            Conversation.UpdateState(new ConversationDoneState(Conversation, this));
        }

        public override Envelope Prepare()
        {
            StockUpdateEventHandler?.Invoke(this, new StockUpdateEventArgs(StockUpdate));
            return null;
        }
    }
}