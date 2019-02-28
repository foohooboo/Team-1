using log4net;
using Shared.Comms.MailService;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Shared.Conversations
{
    public static class ConversationManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static ConcurrentDictionary<string, Conversation> conversations = new ConcurrentDictionary<string, Conversation>();
        private static int count;
        private static int NextConversationCount => Interlocked.Increment(ref count);

        //TODO: Add timeout system for conversations. One idea is to periodically traverse conversations, and remove ones with an old LastUpdateTime.
        //Make sure we log the timeout. -Dsphar 2/21/2019

        public static void Initialize(Func<Envelope, Conversation> conversationFromMessageBuilderFunction)
        {
            ResponderConversationBuilder.SetConversationFromMessageBuilder(conversationFromMessageBuilderFunction);
            PostOffice.SetIncomingMessageHandler(ProcessIncomingMessage);
        }

        public static void AddConversation(Conversation conversation)
        {
            Log.Debug($"{nameof(AddConversation)} (enter)");

            if (conversations.ContainsKey(conversation.ConversationId))
            {
                Log.Error($"Conversation Manager already has a conversation for {conversation.ConversationId}.");
            }
            else
            {
                if (conversations.TryAdd(conversation.ConversationId, conversation))
                    conversation.StartConversation();
                else
                    Log.Error($"Could not add {conversation.ConversationId} to conversations.");
            }

            Log.Debug($"{nameof(AddConversation)} (exit)");
        }

        public static string GenerateNextId(int processID)
        {
            return $"{processID}-{NextConversationCount}";
        }

        public static Conversation ProcessIncomingMessage(Envelope m)
        {
            Log.Debug($"{nameof(ProcessIncomingMessage)} (enter)");

            Conversation conv = null;

            if (conversations.ContainsKey(m.Contents.ConversationID))
            {
                conv = conversations[m.Contents.ConversationID];
                conv.UpdateState(m);
            }
            else
            {
                conv = ResponderConversationBuilder.BuildConversation(m);
                if (conv != null)
                {
                    AddConversation(conv);
                }
            }

            Log.Debug($"{nameof(ProcessIncomingMessage)} (exit)");
            return conv;
        }

        public static void RemoveConversation(string conversationId)
        {
            Log.Debug($"{nameof(RemoveConversation)} (enter)");

            if (conversations.ContainsKey(conversationId))
            {
                conversations.TryRemove(conversationId, out Conversation removed);
            }
            else
            {
                Log.Warn($"Could not find {conversationId} in conversations to remove it.");
            }

            Log.Debug($"{nameof(RemoveConversation)} (exit)");
        }

        public static bool ConversationExists(string conversationId)
        {
            return conversations.ContainsKey(conversationId);
        }
    }
}
