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

            Log.Info($"Started TcpClient on port {((IPEndPoint)myTcpClient.Client.LocalEndPoint).Port}");

            isActive = true;
            new Task(() => ListenForMessages()).Start();
        }

        public TcpClient(System.Net.Sockets.TcpClient client)
        {
            myTcpClient = client;

            Log.Info($"Started TcpClient on port {((IPEndPoint)myTcpClient.Client.LocalEndPoint).Port}");

            isActive = true;
            new Task(() => ListenForMessages()).Start();
        }

        ~TcpClient()
        {
            Close();
        }

        public override void Send(Envelope envelope)
        {
            var messageId = envelope?.Contents?.MessageID ?? null;
            Log.Info($"Sending message {messageId} to {envelope.To}");
            
            byte[] messageBytes = envelope.Contents.Encode();
            byte[] messageLength = BitConverter.GetBytes(messageBytes.Length);

            byte[] bytesToSend = new byte[messageLength.Length + messageBytes.Length];

            System.Buffer.BlockCopy(messageLength,0,bytesToSend,0,bytesToSend.Length);
            System.Buffer.BlockCopy(messageBytes, 0, bytesToSend, messageLength.Length, bytesToSend.Length);

            try
            {
                if (myTcpClient.Connected)
                {
                    if (((IPEndPoint)myTcpClient.Client.LocalEndPoint) != envelope.To)
                    {
                        throw new Exception($"Connected endpoint {myTcpClient.Client.LocalEndPoint.ToString()} does not match envelope address {envelope.To.ToString()}");
                    }
                }
                else
                {
                    myTcpClient.Connect(envelope.To);
                }

                System.Net.Sockets.NetworkStream stream = myTcpClient.GetStream();
                stream.Write(bytesToSend, 0, bytesToSend.Length);
            }
            catch (Exception e)
            {
                Log.Error($"error sending message to {envelope.To}");
                Log.Error(e.Message);
            }
        }

        
        public bool Connect()
        {

            return false;
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
                    waitHandle.Set();
                }
            }
        }

        private Envelope GetIncomingMessages()
        {
            TcpEnvelope newEnvelope = null;

            int incomingData = myTcpClient.Available;

            if (myTcpClient.Connected && (incomingData > 0))
            {
                System.Net.Sockets.NetworkStream stream = myTcpClient.GetStream();

                var receivedBytes = ReceiveBytes(1000, stream);

                var message = MessageFactory.GetMessage(receivedBytes, true);
                Log.Info($"Received {message.GetType()} message from {((IPEndPoint)myTcpClient.Client.LocalEndPoint).Address} via TCP");

                string key = ((IPEndPoint)myTcpClient.Client.LocalEndPoint).Address.ToString();

                newEnvelope = new TcpEnvelope(message)
                {
                    To = (IPEndPoint)myTcpClient.Client.LocalEndPoint,
                    Key = key
                };
            }

            return newEnvelope;
        }

        private byte[] ReceiveBytes(int timeout, System.Net.Sockets.NetworkStream stream)
        {
            byte[] buffer = new byte[256];
            byte[] receivedBytes = new byte[256];
            int receivedBytesIndex = 0;

            stream.Read(buffer, 0, 32);
            int size = BitConverter.ToInt32(buffer, 0);

            while (size > 0)
            {
                try
                {
                    //NOTE: If messages can't be deserialized, check for 1-off errors here!
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        timeout -= 10;

                        if (timeout <= 0)
                        {
                            throw new Exception("Tcp receive timeout");
                        }
                    }
                    else
                    {
                        Array.Copy(buffer, 0, receivedBytes, receivedBytesIndex - 1, bytesRead);
                        size -= bytesRead;
                        receivedBytesIndex += bytesRead;
                    }
                }
                catch (Exception err)
                {
                    Log.Error($"Unexpected exception while receiving data: {err.Message} ");
                }
            }

            stream.Close();
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