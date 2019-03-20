using System;
using System.Net;
using Shared.Comms.Messages;

namespace Shared.Comms.MailService
{
    public class Envelope
    {
        public IPEndPoint To
        {
            get; set;
        }

        public Message Contents
        {
            get; set;
        }

        public Envelope()
        {

        }

        public Envelope(Message message)
        {
            Insert(message);
        }

        public Envelope(Message message, string ip, int port)
        {
            Insert(message);
            To = new IPEndPoint(IPAddress.Parse(ip), port);
            Contents = message;
        }

        public bool HasMessage()
        {
            return Contents != null;
        }

        public void Insert(Message message)
        {
            if (HasMessage())
            {
                throw new Exception("Envelope already has contents");
            }

            Contents = message;
        }

        public Message Remove()
        {
            var contents = Contents;
            Contents = null;

            return contents;
        }
    }
}