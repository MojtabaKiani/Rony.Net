﻿using Rony.Net;
using Rony.Listeners;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Xunit;

namespace Rony.FunctionalTests
{
    public class MockUdpServerTests
    {
        [Fact]
        public async void Server_Should_Return_Correct_Response()
        {
            //Arrange
            const int port = 3000;
            using var server = new MockServer(new UdpServer(port));
            var request = new byte[] { 1, 2, 3 };

            //Act
            server.Mock.Send(request).Receive(x => new byte[] { x[1], 10, x[2] });
            server.Start();
            var client = new UdpClient();
            client.Connect(IPAddress.Parse("127.0.0.1"), port);
            await client.SendAsync(request, request.Length);
            var response = await client.ReceiveAsync();
            client.Close();
            server.Stop();

            //Assert
            Assert.True(response.Buffer.SequenceEqual(new byte[] { 2, 10, 3 }));
        }

        [Theory]
        [InlineData("Match me")]
        [InlineData("12345")]
        [InlineData("****@#")]
        [InlineData("Match me too")]
        public async void Server_Should_Return_Response_To_Any_Request_When_An_Empty_Request_Exists(string request)
        {
            //Arrange
            const int port = 3000;
            using var server = new MockServer(new UdpServer(port));

            //Act
            server.Mock.Send("").Receive("I match everything");
            server.Start();
            var client = new UdpClient();
            client.Connect(IPAddress.Parse("127.0.0.1"), port);
            var requestBytes = request.GetBytes();
            await client.SendAsync(requestBytes, requestBytes.Length);
            var response = await client.ReceiveAsync();
            client.Close();
            server.Stop();

            //Assert
            Assert.Equal("I match everything", response.Buffer.GetString());
        }

        [Theory]
        [InlineData("Match me")]
        [InlineData("12345")]
        [InlineData("****@#")]
        [InlineData("Match me too")]
        public async void Server_Should_Return_Nothing_When_No_Match_Exists(string request)
        {
            //Arrange
            const int port = 3000;
            using var server = new MockServer(new UdpServer(port));

            //Act
            server.Mock.Send("Main Request").Receive(x => new byte[] { x[1], 10, x[2] });
            server.Start();
            var client = new UdpClient();
            client.Connect(IPAddress.Parse("127.0.0.1"), port);
            var requestBytes = request.GetBytes();
            await client.SendAsync(requestBytes, requestBytes.Length);
            var response = await client.ReceiveAsync();
            client.Close();
            server.Stop();

            //Assert
            Assert.Empty(response.Buffer);
        }

        [Fact]
        public async void Server_Should_Return_Correct_Response_On_Multiple_Requests()
        {
            //Arrange
            const int port = 3000;
            using var server = new MockServer(new UdpServer(port));

            //Act
            server.Mock.Send("123").Receive("321");
            server.Mock.Send("ABC").Receive("CBA");
            server.Mock.Send("!@#").Receive("$%^");
            server.Start();
            var client = new UdpClient();
            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            await client.SendAsync("ABC".GetBytes(), 3, endPoint);
            var response = await client.ReceiveAsync();
            client.Close();
            server.Stop();

            //Assert
            Assert.Equal("CBA", response.Buffer.GetString());
        }

        [Fact]
        public async void Server_Should_Return_Correct_Response_On_Many_Request()
        {
            //Arrange
            const int port = 3000;
            using var server = new MockServer(new UdpServer(port));

            //Act
            for (int i = 0; i < 10000; i++)
                server.Mock.Send(i.ToString()).Receive((i + 10000).ToString());
            server.Start();
            for (int i = 0; i < 10000; i++)
            {
                var client = new UdpClient();
                var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                var request = i.ToString().GetBytes();
                await client.SendAsync(request, request.Length, endPoint);
                var response = await client.ReceiveAsync();
                client.Close();

                //Assert
                Assert.Equal((i + 10000).ToString(), response.Buffer.GetString());
            }
            server.Stop();
        }

        [Fact]
        public void Server_Should_Return_Correct_Response_On_Multi_Thread_Request()
        {
            //Arrange
            for (int i = 0; i < 25; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectServer), i.ToString());
            }

            //Act
            async void ConnectServer(object input)
            {
                var header = (string)input;
                using var server = new MockServer(new UdpServer(int.Parse(header)));
                for (int i = 0; i < 2000; i++)
                    server.Mock.Send($"{header}-{i}").Receive($"{header}-{i + 10000}");
                server.Start();
                for (int i = 0; i < 2000; i++)
                {
                    var client = new UdpClient();
                    var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), int.Parse(header));
                    var request = $"{header}-{i}".ToString().GetBytes();
                    await client.SendAsync(request, request.Length, endPoint);
                    var response = await client.ReceiveAsync();
                    client.Close();

                    //Assert
                    Assert.Equal($"{header}-{i + 10000}", response.Buffer.GetString());
                }
                server.Stop();
            }
        }
    }
}
