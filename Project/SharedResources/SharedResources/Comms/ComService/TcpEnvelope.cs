using System;
using System.Net;
using Shared.Comms.Messages;

namespace Shared.Comms.ComService
{
    public class TcpEnvelope : Envelope
    {
        public TcpEnvelope()
        {

        }

        public TcpEnvelope(Message message)
        {
            Insert(message);
        }

        public TcpEnvelope(Message message, string ip, int port)
        {
            Insert(message);
            To = new IPEndPoint(IPAddress.Parse(ip), port);
            Key = To.ToString();
        }

        public TcpEnvelope(Message message, string ip, int port, string key)
        {
            Insert(message);
            To = new IPEndPoint(IPAddress.Parse(ip), port);
            this.Key = key;
        }

        public string Key
        {
            get; set;
        }
    }
}