using System.Net;
using System.Net.Sockets;

namespace Rony.Wrappers
{
    /// <summary>
    /// Wrapper around TcpListener that exposes the Active property
    /// </summary>
    public class TcpListenerWrapper : TcpListener
    {
         public TcpListenerWrapper(IPEndPoint localEp) : base(localEp)
        {
        }

        public TcpListenerWrapper(IPAddress localAddress, int port) : base(localAddress, port)
        {
        }
        public new bool Active => base.Active;
    }
}
