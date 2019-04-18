using log4net;
using Shared.Conversations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;

namespace Shared.Comms.ComService
{
    public static class ComService
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //NOTE: left this to hold both the UDP client as well as any TCP clients
        public static Dictionary<string,Client> Clients = new Dictionary<string, Client>();

        public static bool HasClient()
        {
            return Clients.Count > 0;
        }

        //TODO: tcp?
        //TODO: needs to know if it's adding a upd or tcp client/listener
        public static Client AddClient(string clientId, int localPort)
        {
            Log.Debug($"{nameof(AddClient)} (enter)");

            var box = new UdpClient(localPort);//TODO: change to tcp client
            Clients.Add(clientId, box);

            Log.Debug($"{nameof(AddClient)} (exit)");
            return box;
        }

        //TODO: tcp? Following method not currently used. Would be a good way to get tcp clients if we had a tcp unique identifier
        public static Client GetClient(string address)
        {
            Log.Debug($"{nameof(GetClient)} (enter)");

            Client box = null;

            if (Clients.TryGetValue(address, out Client postBox))
            {
                box = postBox;
            }

            Log.Debug($"{nameof(GetClient)} (exit)");
            return box;
        }

        public static void RemoveClient(string clientId)
        {
            Clients[clientId].Close();
            Clients.Remove(clientId);
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

            if (method == null)
            {
                Log.Warn("Post office was given a null message handler.");
            }
            else if (_incomingMessageHandler != null)
            {
                throw new Exception("Post office already has a message handler.");
            }
            else
            {
                _incomingMessageHandler = new MessageHandler(method);
            }

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
                throw new NullReferenceException($"Please use SetIncomingMessageHandler(delegate) before calling {nameof(HandleIncomingMessage)}");
            }

            var conv = _incomingMessageHandler(e);

            Log.Debug($"{nameof(HandleIncomingMessage)} (exit)");
            return conv;
        }

        public static void Send(string clientId, Envelope env)
        {
            Log.Debug($"{nameof(Send)} (enter)");

            if (!Clients.ContainsKey(clientId))
            {
                Log.Error("Cannot send envelope. No valid client found.");
            }
            else
            {
                try
                {
                    Clients[clientId].Send(env);
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