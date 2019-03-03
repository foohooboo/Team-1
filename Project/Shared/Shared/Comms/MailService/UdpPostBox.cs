using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Shared.Comms.Messages;

namespace Shared.Comms.MailService
{
    public class UdpPostBox : PostBox
    {
        public readonly UdpClient myUdpClient;
        private bool isActive;
        private Task receiveTask;

        public UdpPostBox(string address) : base(address)
        {
            myUdpClient = new UdpClient(LocalEndPoint);
            isActive = true;
            receiveTask = new Task(() => CollectMail());
            
        }

        public override void Send(Envelope envelope)
        {
            byte[] bytesToSend = envelope.Remove().Encode();
            myUdpClient.Send(bytesToSend, bytesToSend.Length, envelope.To);            
        }

        public override void CollectMail()
        {
            while (isActive)
            {
                var envelope = GetEncomingMail();
                if (envelope != null)
                {
                    Mail.Enqueue(envelope);
                    waitHandle.Set();
                }
            }
        }

        private Envelope GetEncomingMail()
        {
            var receivedBytes = ReceiveBytes(1000, out IPEndPoint endPoint);

            if (receivedBytes == null ||
                receivedBytes.Length <= 0)
            {
                return null;
            }

            var message = MessageFactory.GetMessage(receivedBytes);

            var newEnvelope = new Envelope(message)
            {
                To = endPoint
            };

            return newEnvelope;
        }

        private byte[] ReceiveBytes(int timeout, out IPEndPoint endPoint)
        {
            endPoint = null;
            if (myUdpClient is null)
            {
                return null;
            }

            byte[] receivedBytes = null;
            myUdpClient.Client.ReceiveTimeout = timeout;
            endPoint = new IPEndPoint(IPAddress.Any, 0);
            receivedBytes = myUdpClient.Receive(ref endPoint);

            return receivedBytes;
        }
    }
}