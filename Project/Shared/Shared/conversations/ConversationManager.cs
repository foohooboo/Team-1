using log4net;
using Shared.Comms.MailService;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Conversations
{
    public static class ConversationManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static ConcurrentDictionary<string, Conversation> conversations = new ConcurrentDictionary<string, Conversation>();
        private static int count;
        private static int NextConversationCount => Interlocked.Increment(ref count);
        public static bool IsRunning { get; private set; }

        //TODO: Add timeout system for conversations. One idea is to periodically traverse conversations, and remove ones with an old LastUpdateTime.
        //Make sure we log the timeout. -Dsphar 2/21/2019

        public static void Start(Func<Envelope, Conversation> conversationFromMessageBuilderFunction)
        {
            Log.Debug($"{nameof(Start)} (enter)");

            if (!IsRunning)
            {
                ResponderConversationBuilder.SetConversationFromMessageBuilder(conversationFromMessageBuilderFunction);
                PostOffice.SetIncomingMessageHandler(ProcessIncomingMessage);

                IsRunning = true;
                new Task(() => {
                    var timeout = Config.GetInt(Config.DEFAULT_TIMEOUT);
                    while (IsRunning)
                    {
                        foreach(var conv in conversations)
                        {
                            var timeSinceUpdate = (int)(DateTime.Now - conv.Value.LastUpdateTime).TotalMilliseconds;
                            if (timeSinceUpdate > timeout){
                                Log.Warn($"Conversation {conv.Key} timed out.");
                                RemoveConversation(conv.Key);
                            }
                        }
                        Thread.Sleep(timeout);
                    }
                }).Start();
            }
            else
            {
                Log.Warn("ConversationManager already running. Did you try to start it more than once?");
            }

            Log.Debug($"{nameof(Start)} (exit)");
        }

        public static void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                ResponderConversationBuilder.SetConversationFromMessageBuilder(null);
                PostOffice.SetIncomingMessageHandler(null);
                conversations.Clear();
            }
            else
            {
                Log.Warn("ConversationManager already stopped.");
            }
            
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

            if( string.IsNullOrEmpty(m.Contents?.ConversationID))
            {
                Log.Warn("Incoming message does not contain a conversation id.");
            }
            else if (conversations.ContainsKey(m.Contents.ConversationID))
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
