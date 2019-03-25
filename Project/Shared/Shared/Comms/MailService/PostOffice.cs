using log4net;
using Shared.Conversations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;

namespace Shared.Comms.MailService
{
    public static class PostOffice
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Dictionary<string,PostBox> PostBoxes = new Dictionary<string, PostBox>();
        
        public static bool HasPostBox()
        {
            return PostBoxes.Count > 0;
        }

        public static PostBox AddBox(string address)
        {
            Log.Debug($"{nameof(AddBox)} (enter)");

            var box = new UdpPostBox(address);
            PostBoxes.Add(address, box);

            Log.Debug($"{nameof(AddBox)} (exit)");
            return box;
        }

        public static PostBox GetBox(string address)
        {
            if (PostBoxes.TryGetValue(address, out PostBox postBox))
            {
                return postBox;
            }

            return null;
        }

        public static void RemoveBox(string address)
        {
            PostBoxes[address].Close();
            PostBoxes.Remove(address);
        }

        /// <summary>
        /// All unhandled incoming messages should be decoded, wrapped in an envelope, and then sent though this
        /// delegate by calling PostOffice.HandleIncomingMessage(Envelope e). At this point, that particular
        /// message has been handed off and the Post Office does not need to worry about it anymore.
        /// -Dsphar 3/18/2019
        /// </summary>
        public delegate Conversation MessageHandler(Envelope e);
        private static MessageHandler _incomingMessageHandler;
        public static void SetIncomingMessageHandler(MessageHandler method)
        {
            Log.Debug($"{nameof(SetIncomingMessageHandler)} (enter)");

            if (method != null && _incomingMessageHandler != null)
                throw new Exception("IncomingMessageHandler already set.");
            else if (method != null)
                _incomingMessageHandler = new MessageHandler(method);
            else
                Log.Warn("SetIncomingMessageHandler was given a null method.");

            Log.Debug($"{nameof(SetIncomingMessageHandler)} (exit)");
        }

        public static void ClearIncomingMessageHandler()
        {
            _incomingMessageHandler = null;
        }


        public static Conversation HandleIncomingMessage(Envelope e)
        {
            Log.Debug($"{nameof(HandleIncomingMessage)} (enter)");

            if (_incomingMessageHandler == null)
            {
                throw new NullReferenceException($"Please use SetIncomingMessageHandler(handlerFunc) before calling {nameof(HandleIncomingMessage)}");
            }

            var conv = _incomingMessageHandler(e);

            Log.Debug($"{nameof(HandleIncomingMessage)} (exit)");
            return conv;
        }

        public static void Send(Envelope env)
        {
            Log.Debug($"{nameof(Send)} (enter)");

            //TODO: We need to cleanup this hack when we refactor the comm system and add TCP.
            //Ideally, envelopes themselves have enough information for the comm system to determine
            //which communicator to use. That way we can keep this method generic.
            //Dsphar 3/20/19
            if (PostBoxes.Count == 0)
            {
                Log.Error("Cannot send envelope. No valid postbox found.");
            }
            else
            {
                try
                {
                    PostBoxes.Values.First().Send(env);//<<----- HACK ALERT, sends envelope through any arbitrary box.
                }
                catch (Exception e)
                {
                    Log.Error($"Error sending message to {env.To}");
                    Log.Error(e.Message);
                }
            }
            Log.Debug($"{nameof(Send)} (exit)");
        }
    }
}