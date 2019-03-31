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

        public static ConcurrentBag<IPEndPoint> Clients = new ConcurrentBag<IPEndPoint>();

        public static bool TryToAdd(IPEndPoint endPoint)
        {
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (enter)");

            if (Clients.Contains(endPoint))
            {
                Log.Debug($"EndPoint {endPoint.ToString()} is alread in the list");
                return false;
            }

            Clients.Add(endPoint);

            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (exit)");
            return true;
        }
        
        public static bool TryToRemove(IPEndPoint endPoint)
        {
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (enter)");

            if (!Clients.Contains(endPoint))
            {
                Log.Debug($"EndPoint {endPoint.ToString()} is not in the list");
                return false;
            }

            if (!Clients.TryTake(out IPEndPoint removedEndPoint))
            {
                Log.Debug($"Failed to remove endpoint {endPoint}");
                return false;
            }

            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (exit)");
            return true;
        }
    }
}