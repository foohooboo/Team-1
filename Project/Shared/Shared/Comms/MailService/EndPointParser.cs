using log4net;
using System;
using System.Linq;
using System.Net;

namespace Shared.Comms.MailService
{
    public static class EndPointParser
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static IPEndPoint Parse(string address)
        {
            Log.Debug($"{nameof(Parse)} (enter)");

            IPEndPoint endpoint = null;

            var addressParts = address.Split(':');
            if (addressParts.Length == 2)
            {
                int port = ParsePort(addressParts[1]);
                if(IPAddress.TryParse(addressParts[0], out IPAddress ip)){
                    endpoint = new IPEndPoint(ip, port);
                }
                else
                {
                    Log.Warn($"Problem parsing address:{address}");
                }
            }

            Log.Debug($"{nameof(Parse)} (exit)");
            return endpoint;
        }

        public static int ParsePort(string portStr)
        {
            Log.Debug($"{nameof(ParsePort)} (enter)");

            if (!int.TryParse(portStr, out int port))
            {
                Log.Warn($"Problem parsing port string {portStr}");
            }

            Log.Debug($"{nameof(ParsePort)} (enter)");
            return port;
        }
    }
}
