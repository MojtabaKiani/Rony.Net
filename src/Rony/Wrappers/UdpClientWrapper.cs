using System.Net;
using System.Net.Sockets;

namespace Rony.Wrappers
{
    /// <summary>
    /// Wrapper around UdpClient that exposes the Active property
    /// </summary>
    public class UdpClientWrapper : UdpClient
    {
        public UdpClientWrapper(IPEndPoint localEp) : base(localEp)
        {
        }

        public UdpClientWrapper(int port) : base(port)
        {
        }

        public UdpClientWrapper(string hostName, int port) : base(hostName, port)
        {
        }
        public new bool Active => base.Active;
    }
}
