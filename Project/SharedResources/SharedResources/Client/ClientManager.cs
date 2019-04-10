using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using log4net;

namespace Shared.Client
{
    public static class ClientManager
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //I switched this from a bag to dictionary because a bag's "TryTake" doesn't guarantee you removes the desired item.
        //-Dsphar 4/10/2019
        public static ConcurrentDictionary<IPEndPoint,object> Clients = new ConcurrentDictionary<IPEndPoint, object>();

        public static bool TryToAdd(IPEndPoint endPoint)
        {
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (enter)");

            if (Clients.ContainsKey(endPoint))
            {
                Log.Warn($"EndPoint {endPoint.ToString()} is already in the connected client list.");
                return false;
            }

            if(!Clients.TryAdd(endPoint, null))
            {
                Log.Warn($"Something went wrong adding {endPoint.ToString()} to connected client list.");
            }

            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (exit)");
            return true;
        }
        
        public static bool TryToRemove(IPEndPoint endPoint)
        {
            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (enter)");

            if (!Clients.ContainsKey(endPoint))
            {
                Log.Warn($"EndPoint {endPoint.ToString()} is not in the connected client list.");
                return false;
            }

            if (!Clients.TryRemove(endPoint, out object dummy))
            {
                Log.Warn($"Failed to remove endpoint {endPoint} from connected client list.");
                return false;
            }

            Log.Debug($"{System.Reflection.MethodBase.GetCurrentMethod().Name} (exit)");
            return true;
        }
    }
}