using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Shared.Comms.Messages;

namespace Shared.Comms.ComService
{
    public abstract class Client
    {
        protected readonly object clientLock;
        protected readonly AutoResetEvent waitHandle = new AutoResetEvent(false);

        public Client()
        {
            clientLock = new object();
        }

        /// <summary>
        /// Get the port number which this client is bound to locally.
        /// Can only be received after this client has been created.
        /// </summary>
        /// <returns></returns>
        public abstract int getConnectedPort();
        

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