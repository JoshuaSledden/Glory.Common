namespace Glory.Common.Lib.Network.Server
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.ServiceModel.Channels;
    using System.Threading;
    using Glory.Common.Lib.Config;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Implements connection logic for the socket server.
    /// This Architecture uses IOCP in the background for a performant server instance.
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Read and Write Operation Pre-allocation.
        /// We do not allocate buffer space for accepts.
        /// </summary>
        private const int OperationsToPreAllocate = 2;

        /// <summary>
        /// Gets or privately sets the configuration.
        /// </summary>
        public IOptions<ServerConfig> Config { get; private set; }

        /// <summary>
        /// Gets or privately sets the maximum number of simultaneous connections.
        /// </summary>
        public int MaxConnections { get; private set; }

        /// <summary>
        /// Gets or privately sets buffer size to use for each socket I/O operation.
        /// </summary>
        public int ReceiveBufferSize { get; private set; }

        /// <summary>
        /// Gets or privately sets byte count received by the server.
        /// </summary>
        public int TotalBytesRead { get; private set; }

        /// <summary>
        /// Gets or privately sets the total number of clients connected to the server.
        /// </summary>
        public int ConnectionCount { get; private set; }

        /// <summary>
        /// Gets or privately sets represent a large reusable set of buffers for all socket operations.
        /// </summary>
        public BufferManager BufferPool { get; private set; }

        /// <summary>
        /// Gets or privately set the socket used to listen for incoming connection requests.
        /// </summary>
        public Socket ListenSocket { get; private set; }

        /// <summary>
        /// Gets or privately sets a pool of reusable SocketAsyncEventArgsPool objects for write, read and accept socket operations.
        /// </summary>
        public SocketAsyncEventArgsPool IoPool { get; private set; }

        /// <summary>
        /// Gets or privately sets the maximum client limit accepted by the server.
        /// </summary>
        public Semaphore MaxAcceptedClients { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// The server object initializes inactive and needs to be started by calling Init.
        /// </summary>
        /// <param name="config">Injected server configuration object.</param>
        public Server(IOptions<ServerConfig> config)
        {
            Config = config;
            TotalBytesRead = 0;
            ConnectionCount = 0;
            MaxConnections = Config.Value.MaximumConnections;
            ReceiveBufferSize = Config.Value.ReceiveBufferSize;
            MaxAcceptedClients = new Semaphore(MaxConnections, MaxConnections);
        }

        /// <summary>
        /// Initializes the server by pre-allocating reusable buffers and context objects.
        /// </summary>
        public void Init()
        {
            BufferPool = new BufferManager(
                ReceiveBufferSize * MaxConnections * OperationsToPreAllocate,
                ReceiveBufferSize,
                Config);

            SocketAsyncEventArgs readWriteEventArg;

            for (int i = 0; i < MaxConnections; i++)
            {
                readWriteEventArg = new SocketAsyncEventArgs();
                readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                readWriteEventArg.UserToken = new AsyncUserToken();

                BufferPool.SetBuffer(readWriteEventArg);
            }
        }

        /// <summary>
        /// Start the server and begin listening for incoming connection requests.
        /// </summary>
        /// <param name="localEndPoint">The endpoint in which the server will be listening for incoming connection requests.</param>
        public void Start(IPEndPoint localEndPoint)
        {
            // Creates the socket which listens for incoming connections.
            ListenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ListenSocket.Bind(localEndPoint);

            // Start the server with a listen backlog of 100 connections.
            ListenSocket.Listen(100);

            // Post accepts on the listening socket.
            StartAccept(null);
        }

        /// <summary>
        /// Begins an operation to accept a connection request from the client.
        /// </summary>
        /// <param name="acceptEventArg">The context object to use when issuing the accept operation on the server's listening socket.</param>
        public void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                // Socket must be cleared since the context object is being reused.
                acceptEventArg.AcceptSocket = null;
            }

            MaxAcceptedClients.WaitOne();

            bool willRaiseEvent = ListenSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArg);
            }
        }

        /// <summary>
        /// Callback method associated with Socket.AcceptAsync operations.
        /// Is invoked when an accept operation is complete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {

        }
    }
}
