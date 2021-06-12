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
            // Given
            var mockConfig = Options.Create(new ServerConfig());

            var receiveBufferSize = 1024;
            var numConnections = 1000;
            var opsToPreAlloc = 2;

            // When
            var bufferManager = new BufferManager(
                mockConfig,
                receiveBufferSize * numConnections * opsToPreAlloc,
                receiveBufferSize);

            bufferManager.InitBuffer();

            // Then
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
            // Given
            var mockConfig = Options.Create(new ServerConfig
            {
                BufferPoolStartingOffset = currentIndex
            });

            // When
            var bufferManager = new BufferManager(mockConfig, poolSize, chunkSize);
            bufferManager.InitBuffer();

            SocketAsyncEventArgs readWriteEventArg;
            readWriteEventArg = new SocketAsyncEventArgs();

            // Then
            var result = bufferManager.SetBuffer(readWriteEventArg);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ShouldCorrectlyFreeBuffer()
        {

        }
    }
}
