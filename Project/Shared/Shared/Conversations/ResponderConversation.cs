using log4net;
using Shared.Comms.MailService;
using System;

namespace Shared.Conversations
{
    class ResponderConversation : Conversation
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ResponderConversation(Envelope e) : base(e.Contents.ConversationID)
        {
            if (_conversationFromMessageBuilder == null)
            {
                Log.Error("ConversationFromMessageBuilder not set. Ignoring message.");
            }
            else
            {
                var newConversation = _conversationFromMessageBuilder(e);
                if (newConversation != null)
                {
                    ConversationManager.AddConversation(newConversation);
                }
                else
                {
                    Log.Warn($"Unable to create new conversation out of incoming message...\n{e.Contents.Encode()}.");
                }
            }
        }

        private static Func<Envelope, Conversation> _conversationFromMessageBuilder = null;
        public static void SetConversationFromMessageBuilder(Func<Envelope, Conversation> func)
        {
            if (_conversationFromMessageBuilder != null)
                throw new Exception("ConversationFromMessageBuilder can only be set once.");
            else
                _conversationFromMessageBuilder = func;
        }
    }
}
