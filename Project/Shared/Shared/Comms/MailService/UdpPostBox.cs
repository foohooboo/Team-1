using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Shared.Comms.Messages;

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
            isActive = false;
            myUdpClient.Close();
        }

        public void StopListening()
        {
            isActive = false;
        }

        public override void Send(Envelope envelope)
        {
            byte[] bytesToSend = envelope.Remove().Encode();
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
            Log.Debug($"{nameof(GetIncomingMail)} (enter)");

            Envelope newEnvelope = null;

            var receivedBytes = ReceiveBytes(1000, out IPEndPoint endPoint);
            if (receivedBytes != null &&
                receivedBytes.Length > 0)
            {
                var message = MessageFactory.GetMessage(receivedBytes);
                newEnvelope = new Envelope(message)
                {
                    To = endPoint
                };
            }

            Log.Debug($"{nameof(GetIncomingMail)} (exit)");
            return newEnvelope;
        }

        private byte[] ReceiveBytes(int timeout, out IPEndPoint endPoint)
        {
            Log.Debug($"{nameof(ReceiveBytes)} (enter)");

            while (isActive && myUdpClient?.Available <= 0 && timeout > 0)
            {
                Thread.Sleep(10);
                timeout -= 10;
            }

            endPoint = null;
            byte[] receivedBytes = null;

            if (isActive && myUdpClient!=null)
            {
                receivedBytes = myUdpClient.Receive(ref endPoint);
            }
            
            Log.Debug($"{nameof(ReceiveBytes)} (exit)");
            return receivedBytes;
        }
    }
}