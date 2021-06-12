using Glory.Common.Lib.Network.Server;
using Moq;
using System;
using System.Net.Sockets;
using Xunit;

namespace Glory.Common.UnitTests.Network
{
    public class BufferManagerTests
    {
        [Fact]
        public void ShouldCorrectlyInitBuffer()
        {
            // Given
            var receiveBufferSize = 1024;
            var numConnections = 1000;
            var opsToPreAlloc = 2;

            // When
            var bufferManager = new BufferManager(
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

            // When
            var bufferManager = new BufferManager(poolSize, chunkSize, currentIndex);
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
