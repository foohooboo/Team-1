using log4net;
using Shared.Comms.Messages;
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

        public static Conversation InitiateAndStoreConversation<TConversation>(int processID, int portfolioID)
        {
            Conversation conv = Activator.CreateInstance(typeof(TConversation), new object[] { GetNextId(processID,portfolioID) }) as Conversation;
            if(!conversations.TryAdd(conv.Id, conv))
            {
                Log.Error($"Could not add {conv.Id} to conversations.");
            }
            return conv;
        }

        private static string GetNextId(int processID, int portfolioID)
        {
            return $"{processID}-{portfolioID}-{NextConversationCount}";
        }

        public static void ProcessMessage(Message m)
        {
            Log.Debug(string.Format("Enter - {0}", nameof(ProcessMessage)));

            if (conversations.ContainsKey(m.ConversationID))
            {
                conversations[m.ConversationID].UpdateState(m);
            }
            else
            {
                //TODO: If incoming message CAN initiate a conversation, create new conversation and add to conversations dictionary.
                //TODO: If incoming message is NOT known to initiate conversation, log error/warning and drop message.
                throw new NotImplementedException("Creating new conversations from incoming messages has not been implemented yet.");
            }

            Log.Debug(string.Format("Exit - {0}", nameof(ProcessMessage)));
        }

        public static void RemoveConversation(string conversationId)
        {
            Log.Debug(string.Format("Enter - {0}", nameof(RemoveConversation)));

            if (conversations.ContainsKey(conversationId))
            {
                conversations.TryRemove(conversationId, out Conversation removed);
            }
            else
            {
                Log.Warn($"Could not find {conversationId} in conversations to remove it.");
            }

            Log.Debug(string.Format("Exit - {0}", nameof(RemoveConversation)));
        }
    }
}
