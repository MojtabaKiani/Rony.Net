using Rony.Interfaces;
using Rony.Models;
using Rony.Wrappers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Rony.Listeners
{
    public class TcpServer : IListener
    {
        private readonly TcpListenerWrapper _listener;

        public IPAddress Address { get; set; }
        public int Port { get; set; }
        public bool Active => _listener.Active;

        public TcpServer(IPAddress address, int port = 3000)
        {
            _listener = new TcpListenerWrapper(address, port);
            Address = address;
            Port = port;
        }

        public TcpServer(int port = 3000) : this(IPAddress.Parse("127.0.0.1"), port)
        {
        }

        public TcpServer(string address, int port = 3000) : this(IPAddress.Parse(address), port)
        {
        }

        public async Task<Message> ReceiveAsync()
        {
            var client = await _listener.AcceptTcpClientAsync();
            var stream = client.GetStream();
            if (!stream.CanRead) client.Close();
            var buffer = new byte[client.ReceiveBufferSize];
            var sb = new StringBuilder();
            do
            {
                var readBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                sb.Append(Encoding.UTF8.GetString(buffer, 0, readBytes));
            } while (stream.DataAvailable); // Until stream data is available

            return new Message(sb.ToString(), stream);
        }

        public async Task ReplyAsync(string response, object sender)
        {
            await ReplyAsync(response.GetBytes(), sender);
        }

        public async Task ReplyAsync(byte[] response, object sender)
        {
            var stream = (NetworkStream)sender;
            if (response.Length > 0)
                await stream.WriteAsync(response, 0, response.Length);
            stream.Close();
            stream.Dispose();
        }

        public void Start()
        {
            _listener.Start();
        }

        public void Stop()
        {
            _listener.Stop();
        }

        public void Dispose()
        {
            _listener.Stop();
        }
    }
}
