namespace Glory.Common.UnitTests.Network
{
    using Glory.Common.Lib.Config;
    using Glory.Common.Lib.Network.Server;
    using Moq;
    using System;
    using System.Net.Sockets;
    using Xunit;
    using Microsoft.Extensions.Options;

    public class BufferManagerTests
    {
        [Fact]
        public void ShouldCorrectlyInitBuffer()
        {
            /// <given>
            /// The mock configuration is created.
            /// </given>
            var mockConfig = Options.Create(new ServerConfig());

            var receiveBufferSize = 1024;
            var numConnections = 1000;
            var opsToPreAlloc = 2;

            /// <when>
            /// The buffer pool is initialised, we create some pool offsets and set a buffer.
            /// This should ensure the SetBuffer call assigns the readWriteEventArg to a pre-defined offset.
            /// </when>
            var bufferManager = new BufferManager(
                mockConfig,
                receiveBufferSize * numConnections * opsToPreAlloc,
                receiveBufferSize);

            bufferManager.InitBuffer();

            /// <then>
            /// The offset should match the last offset in the stack and the number of remaining free pool offsets should decrease by 1.
            /// </then>
            var bufferSize = bufferManager.ChunkSize;
            Assert.Equal(1024, bufferSize);

            var totalBytes = bufferManager.PoolSize;
            Assert.Equal(2048000, totalBytes);
        }

        [Theory]
        [InlineData(2048, 1024, 0, true)]
        [InlineData(1024, 1024, 0, true)]
        [InlineData(1024, 2048, 0, false)]
        [InlineData(4096, 1024, 0, true)]
        [InlineData(4096, 1024, 4096, false)]
        [InlineData(4096, 3072, 2048, false)]
        [InlineData(1024, 4096, 0, false)]
        public void ShouldReturnCorrectBooleanOnSetBuffer(int poolSize, int chunkSize, int currentIndex, bool expected)
        {
            /// <given>
            /// The mock configuration is created.
            /// </given>
            var mockConfig = Options.Create(new ServerConfig
            {
                BufferPoolStartingOffset = currentIndex
            });

            /// <when>
            /// The buffer pool is initialised, we create some pool offsets and set a buffer.
            /// This should ensure the SetBuffer call assigns the readWriteEventArg to a pre-defined offset.
            /// </when>
            var bufferManager = new BufferManager(mockConfig, poolSize, chunkSize);
            bufferManager.InitBuffer();

            SocketAsyncEventArgs readWriteEventArg;
            readWriteEventArg = new SocketAsyncEventArgs();

            /// <then>
            /// The offset should match the last offset in the stack and the number of remaining free pool offsets should decrease by 1.
            /// </then>
            var result = bufferManager.SetBuffer(readWriteEventArg);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ShouldReturnCorrectBooleanOnSetBufferWithAvailableOffsets()
        {
            /// <given>
            /// The mock configuration is created.
            /// </given>
            var mockConfig = Options.Create(new ServerConfig());

            /// <when>
            /// The buffer pool is initialised, we create some pool offsets and set a buffer.
            /// This should ensure the SetBuffer call assigns the readWriteEventArg to a pre-defined offset.
            /// </when>
            var bufferManager = new BufferManager(mockConfig, 4096, 1024);
            bufferManager.InitBuffer();
            
            bufferManager.FreePoolOffsets.Push(0);
            bufferManager.FreePoolOffsets.Push(1024);
            bufferManager.FreePoolOffsets.Push(2048);

            SocketAsyncEventArgs readWriteEventArg;
            readWriteEventArg = new SocketAsyncEventArgs();

            /// <then>
            /// The offset should match the last offset in the stack and the number of remaining free pool offsets should decrease by 1.
            /// </then>
            var result = bufferManager.SetBuffer(readWriteEventArg);

            Assert.Equal(2048, readWriteEventArg.Offset);
            Assert.Equal(2, bufferManager.FreePoolOffsets.Count);
            Assert.True(result);
            Assert.True(bufferManager.FreePoolOffsets.Contains(0));
            Assert.True(bufferManager.FreePoolOffsets.Contains(1024));
            Assert.False(bufferManager.FreePoolOffsets.Contains(2048));
        }

        [Fact]
        public void ShouldCorrectlyFreeBuffer()
        {
            /// <given>
            /// The mock configuration is created.
            /// </given>
            var mockConfig = Options.Create(new ServerConfig());

            /// <when>
            /// The buffer pool is initialised, we create some pool offsets, set and then free a buffer.
            /// </when>
            var bufferManager = new BufferManager(mockConfig, 4096, 1024);
            bufferManager.InitBuffer();

            bufferManager.FreePoolOffsets.Push(0);
            bufferManager.FreePoolOffsets.Push(1024);
            bufferManager.FreePoolOffsets.Push(2048);

            SocketAsyncEventArgs readWriteEventArg;
            readWriteEventArg = new SocketAsyncEventArgs();

            bufferManager.SetBuffer(readWriteEventArg);
            bufferManager.FreeBuffer(readWriteEventArg);

            /// <then>
            /// The readwriteEventArg buffer should be null.
            /// Additionally the FreePoolOffsets should have the originally allocated offset pushed back in to the stack.
            /// </then>
            Assert.Equal(3, bufferManager.FreePoolOffsets.Count);
            Assert.True(bufferManager.FreePoolOffsets.Contains(0));
            Assert.True(bufferManager.FreePoolOffsets.Contains(1024));
            Assert.True(bufferManager.FreePoolOffsets.Contains(2048));
            Assert.Null(readWriteEventArg.Buffer);
        }
    }
}
