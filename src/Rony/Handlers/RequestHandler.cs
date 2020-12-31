using System;
using System.Collections.Generic;
using System.Linq;

namespace Rony.Handlers
{
    public class RequestHandler
    {
        private string _receiveData;
        public Dictionary<string, string> Configs { get; set; }

        public RequestHandler()
        {
            Configs = new Dictionary<string, string>();
        }

        public RequestHandler Send(string receiveData)
        {
            _receiveData = receiveData;
            return this;
        }

        public RequestHandler Send(byte[] receiveData)
        {
            return Send(receiveData.GetString());
        }

        public void Receive(string response)
        {
            Configs.Add(_receiveData, response);
        }

        public void Receive(byte[] response)
        {
            Receive(response.GetString());
        }

        public void Receive(Func<string, string> func)
        {
            Receive(func(_receiveData));
        }

        public void Receive(Func<byte[], byte[]> func)
        {
            Receive(func(_receiveData.GetBytes()));
        }

        public string Match(string request)
        {
            return Configs.FirstOrDefault(x => x.Key == request).Value;
        }
    }
}
