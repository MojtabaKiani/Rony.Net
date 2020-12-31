[![NuGet version](https://badge.fury.io/nu/Rony.Net.svg)](https://badge.fury.io/nu/Rony.Net)

# Rony.Net
A simple TCP/UDP server for using in test projects which test .net core based projects.

## Problem
When I was working on [Cimon.Net](https://github.com/MojtabaKiani/Cimon.Net) project, I realized that I can't mock sockets with existed libraries, So I changed my project to create fake sockets. But the problem still existed, So I start to write this project and after while I used it in Cimon.Net. 

## Install
You can install `Rony.Net` with [NuGet Package Manager Console](https://www.nuget.org/packages/Rony.Net):
```console
Install-Package Rony.Net
```
Or via the .NET Core command-line interface:
```console
dotnet add package Rony.Net
```    
## Usage
With Rony.Net you can create 3 types of Server :
* TCP Server
* TCP Server with SSL/TLS support
* UDP Server

You can create and run mock servers as below. Port, IP and other settings are configurable via constructors :
```csharp
using var tcpServer = new MockServer(new TcpServer(3000));
tcpSever.Start();
```
```csharp
using var tcpSslServer = new MockServer(new TcpServerSsl(4000, certificateName, SslProtocols.None));
tcpSslServer.Start();
```
```csharp
using var udpServer = new MockServer(new UdpServer(5000));
```
*Please pay attention that UDP server does not need to start, because of its nature.*

Then you can use a normal client to connect and sending request to them, just like below :
```csharp
using var client = new TcpClient();
await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 3000);
```
```csharp
var client = new UdpClient();
client.Connect(IPAddress.Parse("127.0.0.1"), 5000);
```

You can use `server.Mock` to manage Send/Receive data, then server will return configured data, based on sent data:
```csharp
mockServer.Mock.Send("Test String").Receive("Test Response");
mockServer.Mock.Send(new byte[] { 1, 2, 3 }).Receive(new byte[] { 3, 2, 1 });
mockServer.Mock.Send("abcd").Receive(x=> x.ToUpper());
```

You can use `mockServer.Mock` either before or after `mockServer.Run()`. For more details please check Test projects.

## Compile
You need at least Visual Studio 2019 (you can download the Community Edition for free).

## Running the tests
All tests uses Xunit, for running TCP server with SSL/TLS, you should change `_certificateName` field to a certificate name on your machine. Please pay attention that you should have read permission on private key of certificate.
