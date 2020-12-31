using Rony.Listeners;
using Rony.Net;
using System;
using Xunit;

namespace Rony.Tests
{
    public class MockServerTests
    {
        [Fact]
        public void Constructor_Should_Work_Correctly()
        {
            //Arrange
            using var server = new MockServer(new TcpServer(5000));

            //Assert
            Assert.NotNull(server);
            Assert.NotNull(server.Mock);
            Assert.Equal("127.0.0.1", server.Address.ToString());
            Assert.Equal(5000, server.Port);
        }

        [Fact]
        public void Active_Property_Should_Set_Correctly()
        {
            //Arrange
            using var server = new MockServer(new TcpServer(5001));

            //Act
            server.Start();

            //Assert
            Assert.True(server.Active);

            //Act
            server.Stop();

            //Assert
            Assert.False(server.Active);
        }

        [Fact]
        public void Server_Should_Be_Stop_After_Dispose()
        {
            //Arrange
            using var server = new MockServer(new TcpServer(5001));

            //Act
            server.Start();

            //Assert
            Assert.True(server.Active);

            //Act
            server.Dispose();

            //Assert
            Assert.False(server.Active);

        }

        [Fact]
        public void Server_Should_Return_Error_On_Adding_Duplicate_Request()
        {
            //Arrange
            var server = new MockServer(new TcpServer(3000));
            var request = new byte[] { 1, 2, 3 };

            //Act
            server.Mock.Send(request).Receive(new byte[] { 3, 4, 5 });
            
            //Assert
            Assert.Throws<ArgumentException>(()=>server.Mock.Send(request).Receive("test"));
        }
    }
}
