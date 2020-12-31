using Rony.Interfaces;
using Rony.Models;
using Rony.Wrappers;
using System.Net;
using System.Threading.Tasks;

namespace Rony.Listeners
{
    public class UdpServer : IListener
    {
        private UdpClientWrapper _listener;
        private readonly IPEndPoint _endPoint;

        public IPAddress Address { get; set; }
        public int Port { get; set; }
        public bool Active => _listener.Active;

        public UdpServer(IPEndPoint localEp)
        {
            _endPoint = localEp;
            _listener = new UdpClientWrapper(_endPoint);
            Address = localEp.Address;
            Port = localEp.Port;
        }

        public UdpServer(int port = 3000) : this(new IPEndPoint(IPAddress.Any, port))
        {
        }

        public UdpServer(string address, int port = 3000) : this(new IPEndPoint(IPAddress.Parse(address), port))
        {
        }

        public async Task<Message> ReceiveAsync()
        {
            var request = await _listener.ReceiveAsync();
            return new Message(request.Buffer, request.RemoteEndPoint);
        }

        public async Task ReplyAsync(string response, object sender)
        {
            var endPoint = (IPEndPoint)sender;
            await _listener.SendAsync(response.GetBytes(), response.Length, endPoint);
        }

        public void Start()
        {
            
        }

        public void Stop()
        {
            _listener.Dispose();
        }

        public void Dispose()
        {
            _listener.Dispose();
        }
    }
}
