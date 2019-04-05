using log4net;
using Shared.Comms.Messages;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Comms.ComService
{
    public class TcpClient : Client
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool isActive;

        public TcpClient(string address) : base(address)
        {
            //TODO: construct TCP client, left udp below for inspiration
            
            //myUdpClient = new System.Net.Sockets.UdpClient(LocalEndPoint);
            //isActive = true;
            //receiveTask = new Task(() => ListenForMail());
            //receiveTask.Start();
            //Log.Debug($"Started UdpClient on port ${((IPEndPoint)myUdpClient.Client.LocalEndPoint).Port}");
        }       

        ~TcpClient()
        {
            Close();
        }

        public override void Send(Envelope envelope)
        {
            var messageId = envelope?.Contents?.MessageID ?? null;

            Log.Info($"Sending message {messageId} to {envelope.To}");
            byte[] bytesToSend = envelope.Contents.Encode();
            try
            {
                //TODO: Send message over this tcp client
                //Note: if the send for a tcp client is similar to that below, we may be able to move most this method
                //to the base class and only leave the following line in this class?
                //myUdpClient.Send(bytesToSend, bytesToSend.Length, envelope.To);
            }
            catch (Exception e)
            {
                Log.Error($"Error sending message to {envelope.To}");
                Log.Error(e.Message);
            }
        }

        public void ListenForMail()
        {
            while (isActive)
            {
                //Again, if the only thing different here is the def for GetIncomingMail, we may be able to use this entire method to the base class.
                var envelope = GetIncomingMail();
                if (envelope != null)
                {
                    ComService.HandleIncomingMessage(envelope);
                    waitHandle.Set();
                }
            }
        }

        private Envelope GetIncomingMail()
        {
            Envelope newEnvelope = null;

            //TODO: tcp (left udp below for inspiration)

            //var receivedBytes = ReceiveBytes(1000, out IPEndPoint endPoint);
            //if (receivedBytes != null &&
            //    receivedBytes.Length > 0)
            //{
            //    var message = MessageFactory.GetMessage(receivedBytes, true);
            //    newEnvelope = new Envelope(message)
            //    {
            //        To = endPoint
            //    };
            //}
            return newEnvelope;
        }

        private byte[] ReceiveBytes(int timeout, out IPEndPoint endPoint)
        {
            byte[] receivedBytes = null;
            endPoint = null;

            //TODO: tcp (left udp below for inspiration)

            //try
            //{
            //    while (isActive && myUdpClient?.Available <= 0 && timeout > 0)
            //    {
            //        Thread.Sleep(10);
            //        timeout -= 10;
            //    }
            //    if (isActive && myUdpClient?.Available > 0)
            //    {
            //        receivedBytes = myUdpClient.Receive(ref endPoint);
            //        Log.Info($"Received message from {endPoint.ToString()}");
            //    }
            //}
            //catch (Exception err)
            //{
            //    Log.Error($"Unexpected exception while receiving datagram: {err.Message} ");
            //}

            return receivedBytes;
        }

        public override void Close()
        {
            isActive = false;
            //TODO: Close tcp connection if still open
            //myUdpClient?.Close();
        }
    }
}