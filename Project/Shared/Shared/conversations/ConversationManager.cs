using log4net;
using Shared.Comms.MailService;
using Shared.Conversations.SharedStates;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using static Shared.Conversations.ResponderConversationBuilder;

namespace Shared.Conversations
{
    public static class ConversationManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static ConcurrentDictionary<string, Conversation> conversations = new ConcurrentDictionary<string, Conversation>();
        private static int count;
        private static int NextConversationCount => Interlocked.Increment(ref count);
        public static bool IsRunning { get; private set; }

        public static void Start(ConversationBuilder method)
        {
            Log.Debug($"{nameof(Start)} (enter)");

            if (!IsRunning)
            {
                SetConversationBuilder(method);
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
                                var currentState = conv.Value.CurrentState;
                                if (!(currentState is ConversationDoneState))
                                {
                                    Log.Warn($"Raising timeout event for Conversation {conv.Key}.");
                                }
                                currentState.HandleTimeout();
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
            Log.Debug($"{nameof(Stop)} (enter)");

            if (IsRunning)
            {
                IsRunning = false;
                SetConversationBuilder(null);
                PostOffice.SetIncomingMessageHandler(null);
                conversations.Clear();
            }
            else
            {
                Log.Warn("ConversationManager already stopped.");
            }

            Log.Debug($"{nameof(Stop)} (exit)");
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
            Log.Info($"Processing message {m.Contents?.MessageID} for conversation {m.Contents?.ConversationID}.");

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

        public static Conversation GetConversation(string conversationId)
        {
            Log.Debug($"{nameof(GetConversation)} (enter)");

            Conversation conv = null;
            if (conversations.ContainsKey(conversationId))
            {
                conv = conversations[conversationId];
            }
            else
            {
                Log.Warn($"Could not find {conversationId} in conversations.");
            }

            Log.Debug($"{nameof(GetConversation)} (exit)");
            return conv;
        }

        public static bool ConversationExists(string conversationId)
        {
            return conversations.ContainsKey(conversationId);
        }
    }
}
