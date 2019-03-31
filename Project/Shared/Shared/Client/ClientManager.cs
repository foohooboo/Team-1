using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using log4net;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.MarketStructures;
using Shared.PortfolioResources;

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

        public static void UpdateClients(Message message)
        {
            foreach (var client in Clients)
            {
                var env = new Envelope(message, client.Value.Address.ToString(), client.Value.Port);

                PostOffice.Send(env);
            }


        }
    }
}