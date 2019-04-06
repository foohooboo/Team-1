using log4net;
using Shared.Comms.Messages;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Comms.ComService
{
    public class TcpClient : Client
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly System.Net.Sockets.TcpClient myTcpClient;
        private bool isActive;

        /// <summary>
        /// Creates a new TcpCLient and binds it to the provided localPort.
        /// A localPort of 0 will allow the client to bind to any available local port.
        /// </summary>
        /// <param name="localPort"></param>
        public TcpClient(int localPort)
        {
            var bindLocalEndPoint = new IPEndPoint(IPAddress.Any, localPort);
            myTcpClient = new System.Net.Sockets.TcpClient(bindLocalEndPoint);

            //TODO: at some point, we need to connect to the tcp server (TcpListener class) on the other side.

            Log.Info($"Started TcpClient on port ${((IPEndPoint)myTcpClient.Client.LocalEndPoint).Port}");

            isActive = true;
            new Task(() => ListenForMessages()).Start();
        }

        ~TcpClient()
        {
            Close();
        }

        public override void Send(Envelope envelope)
        {
            //TODO: send over TCP (I left udp send for inspiration, although tcp may be different)

            //var messageId = envelope?.Contents?.MessageID ?? null;
            //Log.Info($"Sending message {messageId} to {envelope.To}");
            //byte[] bytesToSend = envelope.Contents.Encode();
            //try
            //{
            //    myTcpClient.Send(bytesToSend, bytesToSend.Length, envelope.To);
            //}
            //catch (Exception e)
            //{
            //    Log.Error($"Error sending message to {envelope.To}");
            //    Log.Error(e.Message);
            //}
        }

        public void ListenForMessages()
        {
            while (isActive)
            {
                var envelope = GetIncomingMessages();
                if (envelope != null)
                {
                    ComService.HandleIncomingMessage(envelope);
                    waitHandle.Set();
                }
            }
        }

        private Envelope GetIncomingMessages()
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
                while (isActive && myTcpClient?.Available <= 0 && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
                if (isActive && myTcpClient?.Available > 0)
                {
                    //TODO: Receive incoming message below (I left udp receive for inspiration, although tcp may be different)
                    //receivedBytes = myUdpClient.Receive(ref endPoint);
                    Log.Info($"Received message from {endPoint.ToString()}");
                }
            }
            catch (Exception err)
            {
                Log.Error($"Unexpected exception while receiving datagram: {err.Message} ");
            }

            return receivedBytes;
        }

        public override void Close()
        {
            isActive = false;
            myTcpClient?.Close();
        }

        public override int getConnectedPort()
        {
            int localPort = -1;

            IPEndPoint endpoint = (IPEndPoint)myTcpClient?.Client?.LocalEndPoint;
            if (endpoint != null)
                localPort = endpoint.Port;

            return localPort;
        }
    }
}