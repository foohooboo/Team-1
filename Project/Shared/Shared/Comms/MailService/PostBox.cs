using System.Collections.Generic;
using System.Net;
using Shared.Comms.Messages;

namespace Shared.Comms.MailService
{
    public abstract class PostBox
    {
        public IPEndPoint EndPoint
        {
            get; private set;
        }

        public Queue<Envelope> Mail
        {
            get; set;
        }

        public PostBox(string address)
        {
            EndPoint = EndPointParser.Parse(address);
            Mail = new Queue<Envelope>();
        }
        
        public bool HasMail()
        {
            return Mail.Count > 0;
        }

        /// <summary>
        /// Used to insert an envelope that is recieved.
        /// </summary>
        /// <param name="envelope">The <see cref="Envelope"/> containing the received <see cref="Message"/>.</param>
        public void Insert(Envelope envelope)
        {
            Mail.Enqueue(envelope);
        }

        public Envelope GetMail()
        {
            if (!HasMail())
            {
                return null;
            }

            return Mail.Dequeue();
        }

        /// <summary>
        /// Used to send an envelope to the address.
        /// </summary>
        /// <param name="envelope">The <see cref="Envelope"/> containing the <see cref="Message"/> to send.</param>
        public abstract void Send(Envelope envelope);
    }
}