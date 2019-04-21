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
            
            byte[] messageBytes = envelope.Contents.Encode();
            byte[] messageLength = BitConverter.GetBytes(messageBytes.Length);

            byte[] bytesToSend = new byte[messageLength.Length + messageBytes.Length];

            Buffer.BlockCopy(messageLength,0,bytesToSend,0, messageLength.Length);
            Buffer.BlockCopy(messageBytes, 0, bytesToSend, messageLength.Length, messageBytes.Length);

            try
            {
                if (!myTcpClient.Connected)
                {
                    //The following COULD cause us to connect to the wrong client, but in our case, by the time
                    //this method gets called, we have already vetted the envelope "To" to this correct TcpClient
                    Connect(envelope.To);
                }

                if (myTcpClient.Connected)
                {
                    System.Net.Sockets.NetworkStream stream = myTcpClient.GetStream();
                    stream.Write(bytesToSend, 0, bytesToSend.Length);
                    ///stream.Close();
                }
                else
                {
                    Log.Error("TCP Message not sent, no connected was made.");
                }

               
            }
            catch (Exception e)
            {
                Log.Error($"error sending message to {envelope.To}");
                Log.Error(e.Message);
            }
        }

        public bool Connect(IPEndPoint distantEnd)
        {
            bool connected = false;
            try
            {
                myTcpClient.Connect(distantEnd);
                connected = myTcpClient.Connected;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
            return connected;
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
                ////Get message
                var stream = myTcpClient.GetStream();
                byte[] bytes = new byte[4];
                stream.Read(bytes, 0, bytes.Length);
                var messageSize = BitConverter.ToInt32(bytes, 0);
                bytes = new byte[messageSize];

                int totalBytesRead = 0;

                while (totalBytesRead != messageSize)
                {
                    totalBytesRead+= stream.Read(bytes, totalBytesRead, bytes.Length-totalBytesRead);
                }

                var message = MessageFactory.GetMessage(bytes);
                string key = ((IPEndPoint)myTcpClient.Client.RemoteEndPoint).ToString();

                newEnvelope = new TcpEnvelope(message)
                {
                    To = (IPEndPoint)myTcpClient.Client.RemoteEndPoint,
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

            //stream.Close();
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