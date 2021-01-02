using Rony.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Rony.Interfaces
{
    public interface IListener : IDisposable
    {
        IPAddress Address { get; }
        int Port { get; }
        bool Active { get; }

        Task<Message> ReceiveAsync();
        Task ReplyAsync(string response, object sender);
        Task ReplyAsync(byte[] response, object sender);
        void Stop();
        void Start();
    }
}
