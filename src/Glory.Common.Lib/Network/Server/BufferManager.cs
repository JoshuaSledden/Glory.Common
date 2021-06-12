namespace Glory.Common.Lib.Network.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Buffer manager creates a single, large buffer.
    /// This buffer can be divided up and assigned to individual SocketAsyncEventArgs objects to be used for a socket I/O operation.
    /// The operations exposed on the BufferManger class are not thread safe.
    /// </summary>
    public class BufferManager : IBufferManager
    {
        /// <summary>
        /// Gets or privately sets the total number of bytes controlled by the pool.
        /// </summary>
        public int PoolSize { get; private set; }

        /// <summary>
        /// Gets or privately sets the byte array buffer maintained by the buffer manager.
        /// </summary>
        public byte[] PoolBuffer { get; private set; }

        /// <summary>
        /// Gets or privately sets the byte size of a chunk of the poolBuffer.
        /// </summary>
        public int ChunkSize { get; private set; }

        /// <summary>
        /// Gets or privately sets a stack representing offset points in the pool of released memory so that it can be reused.
        /// </summary>
        public Stack<int> FreePoolOffsets { get; private set; }

        /// <summary>
        /// Gets or privately sets the current position index within the buffer byte array.
        /// </summary>
        public int CurrentIndex { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferManager"/> class.
        /// </summary>
        /// <param name="poolSize">The byte size of the full pool.</param>
        /// <param name="chunkSize">The allocated chunk size of each pool segment.</param>
        /// <param name="currentIndex">The starting index offset for the buffer pool.</param>
        public BufferManager(int poolSize, int chunkSize, int currentIndex = 0)
        {
            PoolSize = poolSize;
            ChunkSize = chunkSize;
            FreePoolOffsets = new Stack<int>();
            CurrentIndex = currentIndex;
        }

        /// <summary>
        /// Initializes the buffer pool.
        /// </summary>
        public void InitBuffer()
        {
            PoolBuffer = new byte[PoolSize];
        }

        /// <summary>
        /// Assigns a chunk of the buffer pool to a SocketAsyncEventArgs object.
        /// </summary>
        /// <param name="args">A SocketAsyncEventArgs object.</param>
        /// <returns>Bool representing the success state of the set buffer operation.</returns>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            // Check if we have any reusable chunks of the pool available.
            if (FreePoolOffsets.Count > 0)
            {
                args.SetBuffer(PoolBuffer, FreePoolOffsets.Pop(), ChunkSize);
            }
            else
            {
                if ((PoolSize - ChunkSize) < CurrentIndex)
                {
                    return false;
                }

                args.SetBuffer(PoolBuffer, CurrentIndex, ChunkSize);
                CurrentIndex += ChunkSize;
            }

            return true;
        }

        /// <summary>
        /// Frees up a chunk of the memory held by a SocketAsyncEventArgs object and empties the SocketAsyncEventArgs buffer.
        /// Additionally it marks the offset position of the memory chunk being freed to be reused.
        /// </summary>
        /// <param name="args">A SocketAsyncEventArgs object.</param>
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            FreePoolOffsets.Push(args.Offset);
        }
    }
}
