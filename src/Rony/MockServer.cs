using Rony.Handlers;
using Rony.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Rony.Net
{
    public class MockServer : IDisposable
    {
        private readonly IListener _listener;
        private bool _listening = false;
        private readonly object _syncRoot;

        public IPAddress Address => _listener.Address;
        public int Port => _listener.Port;
        public bool Active => _listener.Active;
        public RequestHandler Mock { get; set; }

        public MockServer(IListener listener)
        {
            _syncRoot = new object();
            Mock = new RequestHandler();
            _listener = listener;
        }
        public void Start()
        {
            lock (_syncRoot)
            {
                _listener.Start();
                _listening = true;
            }

            Task.Factory.StartNew(async () =>
            {
                while (_listening)
                {
                    var received = await _listener.ReceiveAsync();
                    var response = Mock.Match(received.BodyString);
                    await _listener.ReplyAsync(response, received.Sender);
                }
            });
        }
        public void Stop()
        {
            if (!_listening) return;
            lock (_syncRoot)
            {
                _listening = false;
                _listener.Stop();
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
