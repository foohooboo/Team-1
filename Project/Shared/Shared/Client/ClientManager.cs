using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using log4net;
using Shared.Comms.MailService;
using Shared.Comms.Messages;

namespace Shared.Client
{
    public static class ClientManager
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ConcurrentDictionary<int, IPEndPoint> Clients = new ConcurrentDictionary<int, IPEndPoint>();

        public static bool TryToAdd(int portfolioID, IPEndPoint endPoint)
        {
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (enter)");

            if (Clients.Keys.Any(portID => portID.Equals(portfolioID)))
            {
                Log.Debug($"Portfolio {portfolioID} is alread in the list");
                return false;
            }

            if (!Clients.TryAdd(portfolioID, endPoint))
            {
                Log.Debug($"Failed to add portfolio for {portfolioID}");
                return false;
            }

            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (exit)");
            return true;
        }
        
        public static bool TryToRemove(int portfolioID)
        {
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (enter)");
            
            if (!Clients.TryRemove(portfolioID, out IPEndPoint endPoint))
            {
                Log.Debug($"Failed to remove client endpoint for portfolio id:: {portfolioID}");
                return false;
            }

            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (exit)");
            return true;
        }

        /// <summary>
        /// The number of messages sent.
        /// This was added to be able to test the messages sent were what was expected.
        /// </summary>
        /// <param name="message">The message to send to the clients.</param>
        /// <returns></returns>
        public static int UpdateClients(Message message)
        {
            var messagesSent = 0;

            foreach (var client in Clients)
            {
                var env = new Envelope(message, client.Value.Address.ToString(), client.Value.Port);

                PostOffice.Send(env);
                messagesSent++;
            }

            return messagesSent;
        }
    }
}