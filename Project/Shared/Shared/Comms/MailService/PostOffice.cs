using log4net;
using Shared.Conversations;
using System;
using System.Collections.Generic;
using System.Net;

namespace Shared.Comms.MailService
{
    public class PostOffice
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Dictionary<EndPoint,PostBox> PostBoxes
        {
            get; set;
        }

        public PostOffice()
        {
            PostBoxes = new Dictionary<EndPoint, PostBox>();
        }

        public bool HasPostBox()
        {
            return PostBoxes.Count > 0;
        }

        public void AddBox(EndPoint address)
        {
            PostBoxes.Add(address, new PostBox(address));
        }

        public PostBox GetBox(EndPoint address)
        {
            if (PostBoxes.TryGetValue(address, out PostBox postBox))
            {
                return postBox;
            }

            return null;
        }

        public void RemoveBox(EndPoint address)
        {
            PostBoxes.Remove(address);
        }

        /// <summary>
        /// To achieve dependency inversion, the _incomingMessaegHandler will be set by a higher-level object. As such,
        /// any incoming messages that are received should be decoded, wrapped in an envelope, and then sent though this
        /// static object by calling PostOffice.HandleIncomingMessage(Envelope e) at which point that particular
        /// message has been handed off and the Post Office does not need to worry about it anymore.
        /// -Dsphar 2/27/2019
        /// </summary>
        private static Func<Envelope, Conversation> _incomingMessageHandler = null;
        public static void SetIncomingMessageHandler(Func<Envelope, Conversation> func)
        {
            Log.Debug($"{nameof(SetIncomingMessageHandler)} (enter)");

            if (_incomingMessageHandler != null)
                throw new Exception("IncomingMessageHandler can only be set once.");
            else
                _incomingMessageHandler = func;

            Log.Debug($"{nameof(SetIncomingMessageHandler)} (exit)");
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
    }
}