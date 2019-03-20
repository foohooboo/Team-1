using log4net;
using Shared.Comms.Messages;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Comms.MailService
{
    public class UdpPostBox : PostBox
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly UdpClient myUdpClient;
        private bool isActive;
        private Task receiveTask;

        public UdpPostBox(string address) : base(address)
        {
            myUdpClient = new UdpClient(LocalEndPoint);
            isActive = true;
            receiveTask = new Task(() => CollectMail());
            receiveTask.Start();
        }

        ~UdpPostBox()
        {
            Close();
        }

        public override void Send(Envelope envelope)
        {
            Log.Info($"Sending message {envelope?.Contents?.MessageID} to {envelope.To}");
            byte[] bytesToSend = envelope.Contents.Encode();
            try
            {
                myUdpClient.Send(bytesToSend, bytesToSend.Length, envelope.To);
            }
            catch (Exception e)
            {
                Log.Error($"Error sending message to {envelope.To}");
                Log.Error(e.Message);
            }
        }

        public override void CollectMail()
        {

            while (isActive)
            {
                var envelope = GetIncomingMail();
                if (envelope != null)
                {
                    Mail.Enqueue(envelope);
                    PostOffice.HandleIncomingMessage(envelope);
                    waitHandle.Set();
                }
            }
        }

        private Envelope GetIncomingMail()
        {
            Envelope newEnvelope = null;

            var receivedBytes = ReceiveBytes(1000, out IPEndPoint endPoint);
            if (receivedBytes != null &&
                receivedBytes.Length > 0)
            {
                var message = MessageFactory.GetMessage(receivedBytes, true);
                newEnvelope = new Envelope(message)
                {
                    To = endPoint
                };
            }
            return newEnvelope;
        }

        private byte[] ReceiveBytes(int timeout, out IPEndPoint endPoint)
        {
            byte[] receivedBytes = null;
            endPoint = null;

            try
            {
                while (isActive && myUdpClient?.Available <= 0 && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
                if (isActive && myUdpClient?.Available > 0)
                {
                    receivedBytes = myUdpClient.Receive(ref endPoint);
                    Log.Info($"Received message from {endPoint.ToString()}");
                }
            }
            catch (Exception err)
            {
                Log.Warn($"Unexpected exception while receiving datagram: {err.Message} ");
            }

            return receivedBytes;
        }

        public override void Close()
        {
            isActive = false;
            myUdpClient?.Close();
        }
    }
}