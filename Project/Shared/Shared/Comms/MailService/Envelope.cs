using System;
using System.Net;
using Shared.Comms.Messages;

namespace Shared.Comms.MailService
{
    public class Envelope
    {
        public EndPoint To
        {
            get; set;
        }

        public EndPoint From
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