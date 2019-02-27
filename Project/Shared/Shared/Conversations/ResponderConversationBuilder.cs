using log4net;
using Shared.Comms.MailService;
using System;

namespace Shared.Conversations
{
    public static class ResponderConversationBuilder
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Conversation BuildConversation(Envelope e)
        {
            Conversation conv = null;

            if (_conversationFromMessageBuilder == null)
            {
                Log.Error("ConversationFromMessageBuilder not set. Ignoring message.");
            }
            else
            {
                conv = _conversationFromMessageBuilder(e);
                if (conv == null)
                {
                    Log.Warn($"ConversationFromMessageBuilder failed to create new conversation out of incoming message...\n{e.Contents.Encode()}.");
                }
            }

            return conv;
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
