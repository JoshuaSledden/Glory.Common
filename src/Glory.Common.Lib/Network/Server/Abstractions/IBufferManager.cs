namespace Glory.Common.Lib.Network.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface representing a BufferManager object.
    /// </summary>
    public interface IBufferManager
    {
        /// <summary>
        /// Gets the total number of bytes controlled by the pool.
        /// </summary>
        int CurrentIndex { get; }

        /// <summary>
        /// Gets the byte array buffer maintained by the buffer manager.
        /// </summary>
        int PoolSize { get; }

        /// <summary>
        /// Gets the byte size of a chunk of the poolBuffer.
        /// </summary>
        int ChunkSize { get; }

        /// <summary>
        /// Initializes the buffer pool.
        /// </summary>
        void InitBuffer();

        /// <summary>
        /// Assigns a chunk of the buffer pool to a SocketAsyncEventArgs object.
        /// </summary>
        /// <param name="args">A SocketAsyncEventArgs object.</param>
        /// <returns>Bool representing the success state of the set buffer operation.</returns>
        bool SetBuffer(SocketAsyncEventArgs args);

        /// <summary>
        /// Frees up a chunk of the memory held by a SocketAsyncEventArgs object and empties the SocketAsyncEventArgs buffer.
        /// Additionally it marks the offset position of the memory chunk being freed to be reused.
        /// </summary>
        /// <param name="args">A SocketAsyncEventArgs object.</param>
        void FreeBuffer(SocketAsyncEventArgs args);
    }
}
