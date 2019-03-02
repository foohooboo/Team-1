using System;
using System.Linq;
using System.Net;

namespace Shared.Comms.MailService
{
    public static class EndPointParser
    {
        public static IPEndPoint Parse(string address)
        {
            if (String.IsNullOrWhiteSpace(address))
            {
                return null;
            }

            var addressParts = address.Split(':');
            
            if (addressParts.Length != 2 ||
                String.IsNullOrWhiteSpace(addressParts[0]) ||
                String.IsNullOrWhiteSpace(addressParts[1]))
            {
                return null;
            }
            
            return new IPEndPoint(ParseAddress(addressParts[0]), ParsePort(addressParts[1]));
            
        }

        public static IPAddress ParseAddress(string hostname)
        {
            var addressList = Dns.GetHostAddresses(hostname);

            return addressList.First(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        }

        public static int ParsePort(string portStr)
        {
            if (String.IsNullOrWhiteSpace(portStr))
            {
                return 0;
            }

            int.TryParse(portStr, out int port);

            return port;
        }
    }
}
