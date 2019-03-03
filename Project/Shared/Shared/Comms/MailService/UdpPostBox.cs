using System.Net;
using System.Net.Sockets;

namespace Shared.Comms.MailService
{
    public class UdpPostBox : PostBox
    {
        public readonly UdpClient myUdpClient;
        public IPEndPoint localEndpoint
        {
            get; set;
        }

        public UdpPostBox(string address) : base(address)
        {
            localEndpoint = new IPEndPoint(IPAddress.Any, 0);
            myUdpClient = new UdpClient(localEndpoint);
        }

        public override void Send(Envelope envelope)
        {

        }
    }
}