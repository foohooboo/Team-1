using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Shared.Comms.Messages;

namespace Shared.Comms.ComService
{
    public abstract class PostBox
    {
        private readonly object boxLock;
        protected readonly AutoResetEvent waitHandle = new AutoResetEvent(false);

        public IPEndPoint LocalEndPoint
        {
            get; private set;
        }

        public PostBox(string address)
        {
            boxLock = new object();
            LocalEndPoint = EndPointParser.Parse(address);
        }

        /// <summary>
        /// Used to shutdown the socket.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Used to send an envelope to the address.
        /// </summary>
        /// <param name="envelope">The <see cref="Envelope"/> containing the <see cref="Message"/> to send.</param>
        public abstract void Send(Envelope envelope);
    }
}