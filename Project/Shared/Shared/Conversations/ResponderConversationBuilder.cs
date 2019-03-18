using log4net;
using Shared.Comms.MailService;
using System;

namespace Shared.Conversations
{
    public static class ResponderConversationBuilder
    {
        //Note: Please rout all incoming messages through the ConversationManager.ProcessIncomingMessage 
        //method and not this class. ConversationManager.ProcessIncomingMessage will use this class when/if necessary.
        //Having said that, the application layer still needs to set this _conversationFromMessageBuilder on startup.
        //I suppose we could rout that through the ConversationManager if we wanted so the application layer doesn't
        //need to set it directly?...
        //-Dsphar 2/27/19

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Conversation BuildConversation(Envelope e)
        {
            Conversation conv = null;

            if (conversationBuilder.GetInvocationList().Length == 0)
            {
                Log.Error("ConversationBuilder not set. Ignoring message.");
            }
            else
            {
                conv = conversationBuilder(e);
                if (conv == null)
                {
                    Log.Warn($"ConversationFromMessageBuilder failed to create new conversation from incoming message...\n{e.Contents}.");
                }
            }

            return conv;
        }

        public delegate Conversation ConversationBuilder(Envelope e);
        private static ConversationBuilder conversationBuilder;
        public static void SetConversationBuilder(ConversationBuilder method)
        {
            if (conversationBuilder?.GetInvocationList().Length > 0)
                throw new Exception("ConversationFromMessageBuilder can only be set once.");
            else if (method != null)
                conversationBuilder = new ConversationBuilder(method);
            else
                Log.Warn($"SetConversationBuilder was given a null ConversationBuilder.");
        }
    }
}
