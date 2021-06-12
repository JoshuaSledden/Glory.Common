namespace Glory.Common.Lib.Config
{
    /// <summary>
    /// Configuration options for the Server.
    /// </summary>
    public class ServerConfig
    {
        /// <summary>
        /// Gets or sets the maximum number of simultaneous client connections to a server instance.
        /// </summary>
        public int MaximumConnections { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the maximum buffer size for each I/O operation.
        /// </summary>
        public int ReceiveBufferSize { get; set; } = 1024;

        /// <summary>
        /// Gets or sets the listen backlog size for when a server starts.
        /// </summary>
        public int ListenBacklog { get; set; } = 100;

        /// <summary>
        /// Gets or sets the current position index within the buffer byte array.
        /// </summary>
        public int BufferPoolStartingOffset { get; set; } = 0;
    }
}
