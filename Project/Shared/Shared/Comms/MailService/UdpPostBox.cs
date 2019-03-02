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
            if (Peers == null || Peers.Count <= 0)
                return;

            var messageBytes = envelope.Remove().Encode();

            foreach (var endPoint in Peers)
            {
                var result = myUdpClient.Send(messageBytes, messageBytes.Length, endPoint);
            }
        }
    }
}
