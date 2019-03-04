using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Shared.Comms.Messages;

namespace Shared.Comms.MailService
{
    public abstract class PostBox
    {
        private readonly object boxLock;
        protected readonly AutoResetEvent waitHandle = new AutoResetEvent(false);

        public IPEndPoint LocalEndPoint
        {
            get; private set;
        }

        public ConcurrentQueue<Envelope> Mail
        {
            get; set;
        }

        public PostBox(string address)
        {
            boxLock = new object();
            LocalEndPoint = EndPointParser.Parse(address);
            Mail = new ConcurrentQueue<Envelope>();
        }
        
        public bool HasMail()
        {
            return Mail.Count > 0;
        }

        /// <summary>
        /// Used to insert an envelope that is recieved.
        /// </summary>
        public abstract void CollectMail();

        public Envelope GetMail()
        {
            var startTime = DateTime.Now;

            Envelope envelope = null;

            while (envelope is null &&
                   DateTime.Now.Subtract(startTime).TotalMilliseconds < 1000)
            {
                if (!HasMail())
                {
                    waitHandle.WaitOne(1000);
                }

                if (!Mail.TryDequeue(out envelope))
                {
                    envelope = null;
                }
            }

            return envelope;
        }

        /// <summary>
        /// Used to send an envelope to the address.
        /// </summary>
        /// <param name="envelope">The <see cref="Envelope"/> containing the <see cref="Message"/> to send.</param>
        public abstract void Send(Envelope envelope);
    }
}