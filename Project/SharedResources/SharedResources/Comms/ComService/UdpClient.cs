using log4net;
using Shared.Comms.Messages;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Comms.ComService
{
    public class UdpClient : Client
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly System.Net.Sockets.UdpClient myUdpClient;
        private bool isActive;

        /// <summary>
        /// Creates a new UdpClient and binds it to the provided localPort.
        /// A localPort of 0 will allow the client to bind to any available local port.
        /// </summary>
        /// <param name="localPort"></param>
        public UdpClient(int localPort)
        {
            var bindLocalEndPoint = new IPEndPoint(IPAddress.Any, localPort);
            myUdpClient = new System.Net.Sockets.UdpClient(bindLocalEndPoint);

            Log.Info($"Started UdpClient on port ${((IPEndPoint)myUdpClient.Client.LocalEndPoint).Port}");

            isActive = true;
            new Task(() => ListenForMessages()).Start();
        }

        ~UdpClient()
        {
            Close();
        }

        public override void Send(Envelope envelope)
        {
            var messageId = envelope?.Contents?.MessageID ?? null;
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

        public void ListenForMessages()
        {
            while (isActive)
            {
                var envelope = GetIncomingMessages();
                if (envelope != null)
                {
                    try
                    {
                        ComService.HandleIncomingMessage(envelope);
                        
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Problem encountered while handling incoming message...");
                        Log.Error(e);
                    }

                    waitHandle.Set();//TODO: <- I don't think this is needed anymore?? -Dsphar 4/7/2019
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
                while (isActive && myUdpClient?.Available <= 0 && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
                if (isActive && myUdpClient?.Available > 0)
                {
                    receivedBytes = myUdpClient.Receive(ref endPoint);
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
            myUdpClient?.Close();
        }

        public override int getConnectedPort()
        {
            int localPort = -1;

            IPEndPoint endpoint = (IPEndPoint)myUdpClient?.Client?.LocalEndPoint;
            if (endpoint != null)
                localPort = endpoint.Port;

            return localPort;
        }
    }
}