using Rony.Interfaces;
using Rony.Models;
using Rony.Wrappers;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Rony.Listeners
{
    public class TcpServerSsl : IListener
    {
        private readonly TcpListenerWrapper _listener;
        private readonly string _certificateName;
        private readonly SslProtocols _protocol;

        public IPAddress Address { get; set; }
        public int Port { get; set; }
        public bool Active => _listener.Active;

        public TcpServerSsl(IPAddress address, int port, string certificateName, SslProtocols protocol)
        {
            _certificateName = certificateName;
            _protocol = protocol;
            _listener = new TcpListenerWrapper(address, port);
            Address = address;
            Port = port;
        }

        public TcpServerSsl(int port, string certificateName, SslProtocols protocol)
            : this(IPAddress.Parse("127.0.0.1"), port, certificateName, protocol)
        {
        }

        public TcpServerSsl(string address, int port, string certificateName, SslProtocols protocol)
            : this(IPAddress.Parse(address), port, certificateName, protocol)
        {
        }

        public async Task<Message> ReceiveAsync()
        {
            var client = await _listener.AcceptTcpClientAsync();
            var sslStream = new SslStream(client.GetStream());
            await sslStream.AuthenticateAsServerAsync(GetServerCert(_certificateName), false, _protocol, true);
            if (!sslStream.CanRead) client.Close();
            var buffer = new byte[client.ReceiveBufferSize];
            var readBytes = await sslStream.ReadAsync(buffer, 0, buffer.Length);
            var receivedData = buffer.Take(readBytes).ToArray().GetString();

            return new Message(receivedData, sslStream);
        }

        public async Task ReplyAsync(string response, object sender)
        {
            var sslStream = (SslStream)sender;
            if (!string.IsNullOrEmpty(response))
            {
                var responseBytes = response.GetBytes();
                await sslStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
            sslStream.Close();
            sslStream.Dispose();
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

        private static X509Certificate GetServerCert(string subjectName)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            X509CertificateCollection certificate = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);
            return certificate.Count > 0 ? certificate[0] : null;
        }
    }
}
