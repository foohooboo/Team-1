using System.Collections.Generic;
using System.Net;

namespace Shared.Comms.MailService
{
    public class PostBox
    {
        public EndPoint OutgoingAddress
        {
            get; private set;
        }

        public EndPoint IncomingAddress
        {
            get; private set;
        }

        public Queue<Envelope> Mail
        {
            get; set;
        }

        public PostBox(EndPoint endpoint)
        {

        }

        public void Insert(Envelope envelope)
        {
            Mail.Enqueue(envelope);
        }

        public void SendMessage(Envelope envelope)
        {
            envelope.From = OutgoingAddress;

            // Send to Outgoing Address.
        }
    }
}