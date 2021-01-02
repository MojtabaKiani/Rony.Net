using Rony.Listeners;
using Rony.Net;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Xunit;

namespace Rony.FunctionalTests
{
    public class MockTcpServerTests
    {
        [Fact]
        public async void Server_Should_Return_Correct_Response()
        {
            //Arrange
            using var server = new MockServer(new TcpServer(3000));
            var request = new byte[] { 1, 2, 3 };
            using var client = new TcpClient();

            //Act
            server.Mock.Send(request).Receive(x => new byte[] { x[1], 10, x[2] });
            server.Start();
            await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 3000);
            using var stream = client.GetStream();
            await stream.WriteAsync(request, 0, request.Length);
            var response = new byte[client.ReceiveBufferSize];
            var bytes = await stream.ReadAsync(response, 0, response.Length);
    51            client.Close();
            server.Stop();

            //Assert
            Assert.True(response.Take(bytes).SequenceEqual(new byte[] { 2, 10, 3 }));
        }

        [Theory]
        [InlineData("Match me")]
        [InlineData("12345")]
        [InlineData("****@#")]
        [InlineData("Match me too")]
        public async void Server_Should_Return_Response_To_Any_Request_When_An_Empty_Request_Exists(string request)
        {
            //Arrange
            using var server = new MockServer(new TcpServer(3000));
            using var client = new TcpClient();

            //Act
            server.Mock.Send("").Receive("I match everything");
            server.Start();
            await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 3000);
            using var stream = client.GetStream();
            var requestBytes = request.GetBytes();
            await stream.WriteAsync(requestBytes, 0, requestBytes.Length);
            var response = new byte[client.ReceiveBufferSize];
            var bytes = await stream.ReadAsync(response, 0, response.Length);
            client.Close();
            server.Stop();

            //Assert
            Assert.Equal("I match everything", response.Take(bytes).ToArray().GetString());
        }

        [Theory]
        [InlineData("Match me")]
        [InlineData("12345")]
        [InlineData("****@#")]
        [InlineData("Match me too")]
        public async void Server_Should_Return_Nothing_When_No_Match_Exists(string request)
        {
            //Arrange
            using var server = new MockServer(new TcpServer(3000));
            using var client = new TcpClient();

            //Act
            server.Mock.Send("Main Request").Receive(x => new byte[] { x[1], 10, x[2] });
            server.Start();
            await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 3000);
            using var stream = client.GetStream();
            var requestBytes = request.GetBytes();
            await stream.WriteAsync(requestBytes, 0, requestBytes.Length);
            var response = new byte[client.ReceiveBufferSize];
            var bytes = await stream.ReadAsync(response, 0, response.Length);
            client.Close();
            server.Stop();

            //Assert
            Assert.Equal(0,bytes);
        }

        [Fact]
        public async void Server_Should_Return_Correct_Response_On_Multiple_Requests()
        {
            //Arrange
            using var server = new MockServer(new TcpServer(3000));
            using var client = new TcpClient();

            //Act
            server.Mock.Send("123").Receive("321");
            server.Mock.Send("ABC").Receive("CBA");
            server.Mock.Send("!@#").Receive("$%^");
            server.Start();
            await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 3000);
            using var stream = client.GetStream();
            await stream.WriteAsync("ABC".GetBytes(), 0, 3);
            var response = new byte[client.ReceiveBufferSize];
            var bytes = stream.Read(response, 0, response.Length);
            client.Close();
            server.Stop();

            //Assert
            Assert.Equal("CBA", response.Take(bytes).ToArray().GetString());
        }

        [Fact]
        public async void Server_Should_Return_Correct_Response_On_Many_Request()
        {
            //Arrange
            using var server = new MockServer(new TcpServer(3000));

            //Act
            for (int i = 0; i < 10000; i++)
                server.Mock.Send(i.ToString()).Receive((i + 10000).ToString());
            server.Start();
            for (int i = 0; i < 10000; i++)
            {
                using var client = new TcpClient();
                await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 3000);
                using var stream = client.GetStream();
                var response = new byte[client.ReceiveBufferSize];
                var request = i.ToString().GetBytes();
                await stream.WriteAsync(request, 0, request.Length);
                var bytes = stream.Read(response, 0, response.Length);
                client.Close();

                //Assert
                Assert.Equal((i + 10000).ToString(), response.Take(bytes).ToArray().GetString());
            }
            server.Stop();
        }

        [Fact]
        public void Server_Should_Return_Correct_Response_On_Multi_Thread_Request()
        {
            //Act
            for (int i = 0; i < 100; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectServer), i.ToString());
            }

            async void ConnectServer(object input)
            {
                var header = (string)input;
                using var server = new MockServer(new TcpServer(int.Parse(header)));
                for (int i = 0; i < 3000; i++)
                    server.Mock.Send($"{header}-{i}").Receive($"{header}-{i + 10000}");
                server.Start();
                for (int i = 0; i < 3000; i++)
                {
                    var client = new TcpClient();
                    await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), int.Parse(header));
                    using var stream = client.GetStream();
                    var response = new byte[client.ReceiveBufferSize];
                    var request = $"{header}-{i}".ToString().GetBytes();
                    await stream.WriteAsync(request, 0, request.Length);
                    var bytes = stream.Read(response, 0, response.Length);
                    client.Close();

                    //Assert
                    Assert.Equal($"{header}-{i + 10000}", response.Take(bytes).ToArray().GetString());
                }
                server.Stop();
            }
        }
    }
}
