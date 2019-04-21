using log4net;
using Shared.Comms.Messages;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


namespace Shared.Comms.ComService
{
    /// <summary>
    /// Listens on a tcp port specified in the config. Does not respond. 
    /// </summary>
    public class TcpListenerClient : Client
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly System.Net.Sockets.TcpListener myTcpListenerClient;
        private bool isActive;

        /// <summary>
        /// Creates a new TcpListener and binds it to the provided localPort.
        /// A localPort of 0 will allow the client to bind to any available local port.
        /// NOTE: If you bind to any port, you'll need to be able to know which one it found if you want to send anything to it.
        /// </summary>
        /// <param name="localPort"></param>
        public TcpListenerClient(int localPort)
        {
            var bindLocalEndPoint = new IPEndPoint(IPAddress.Any, localPort);
            myTcpListenerClient = new System.Net.Sockets.TcpListener(bindLocalEndPoint);

            myTcpListenerClient.Start();

            Log.Info($"Started TcpListener on port {((IPEndPoint)myTcpListenerClient.LocalEndpoint).Port}");

            isActive = true;
            new Task(() => ListenForMessages()).Start();
        }

        ~TcpListenerClient()
        {
            Close();
        }

        //NOTE: TcpListener's don't send
        public override void Send(Envelope envelope)
        {
            throw new Exception("TcpListener cannot send");
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

            //Docs say that AcceptTcpClient() is a blocking call, so it won't just spin it's wheels. There is an asynch, AcceptTcpClientAsync. 
            Log.Info($"Waiting for TCP connection on port ${((IPEndPoint)myTcpListenerClient.LocalEndpoint).Port}");

            System.Net.Sockets.TcpClient client = myTcpListenerClient.AcceptTcpClient();

            try
            {
                Log.Info($"Incoming TCP Connection established with {((IPEndPoint)client.Client.RemoteEndPoint).ToString()}");
                ComService.AddTcpClient(client);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return newEnvelope;
        }

        //unused in a TcpListener, because messages actually handled in TcpClient which spins off this listener on connection made.
        private byte[] ReceiveBytes(int timeout, System.Net.Sockets.NetworkStream stream)
        {
            return new byte[256];
        }

        public override void Close()
        {
            isActive = false;
            myTcpListenerClient?.Stop();
        }

        public override int getConnectedPort()
        {
            int localPort = -1;

            IPEndPoint endpoint = (IPEndPoint)myTcpListenerClient?.LocalEndpoint;
            if (endpoint != null)
                localPort = endpoint.Port;

            return localPort;
        }
    }
}