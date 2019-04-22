using log4net;
using Shared.Conversations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Collections.Concurrent;

namespace Shared.Comms.ComService
{
    public static class ComService
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //NOTE: left this to hold both the UDP client as well as any TCP clients
        public static ConcurrentDictionary<string,Client> Clients = new ConcurrentDictionary<string, Client>();

        public static bool HasClient()
        {
            return Clients.Count > 0;
        }

        public static Client AddUdpClient(string clientId, int localPort)
        {
            Log.Debug($"{nameof(AddUdpClient)} (enter)");

            var box = new UdpClient(localPort);
            Clients.TryAdd(clientId, box);

            Log.Debug($"{nameof(AddUdpClient)} (exit)");
            return box;
        }

        public static Client AddTcpListener(string clientId, int localPort)
        {
            Log.Debug($"{nameof(AddTcpClient)} (enter)");

            var box = new TcpListenerClient(localPort);
            Clients.TryAdd(clientId, box);

            Log.Debug($"{nameof(AddTcpClient)} (exit)");
            return box;
        }

        public static TcpClient AddTcpClient(int localPort, IPEndPoint remoteEndpoint)
        {
            Log.Debug($"{nameof(AddTcpClient)} (enter)");

            var box = new TcpClient(localPort);

            //NOTE: used to connect at the time of the first send, but in order to store the box by it's remote ep
            //we need to connect immediately.
            if (box.Connect(remoteEndpoint))
            {
                Clients.TryAdd(((IPEndPoint)box.myTcpClient.Client.RemoteEndPoint).ToString(), box);
            }
            else
            {
                box = null;
                Log.Error("failed to connect TcpClient to remote endpoint.");
            }

            Log.Debug($"{nameof(AddTcpClient)} (exit)");

            
            return box;
        }

        public static Client AddTcpClient(System.Net.Sockets.TcpClient client)
        {
            Log.Debug($"{nameof(AddTcpClient)} (enter)");

            var box = new TcpClient(client);
            string key = ((IPEndPoint)client.Client.RemoteEndPoint).ToString();

            Clients.TryAdd(key, box);

            Log.Debug($"{nameof(AddTcpClient)} (exit)");
            return box;
        }

        //TODO: tcp? Following method not currently used. Would be a good way to get tcp clients if we had a tcp unique identifier
        //NOTE: Certain, default tcp identifiers are now in the config files, others will simply use a string of their endpoint as their key
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
            if (Clients.ContainsKey(clientId)){
                Clients[clientId].Close();
                Clients.TryRemove(clientId, out Client client);
            }
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