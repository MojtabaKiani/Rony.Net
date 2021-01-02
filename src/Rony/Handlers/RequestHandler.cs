using Rony.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rony.Handlers
{
    public class RequestHandler
    {
        private string _receiveData;
        public Dictionary<string, Config> Configs { get; set; }

        public RequestHandler()
        {
            Configs = new Dictionary<string, Config>();
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
            Configs.Add(_receiveData, new Config(response));
        }

        public void Receive(byte[] response)
        {
            Configs.Add(_receiveData, new Config(response.GetString()));
        }

        public void Receive(Func<string, string> func)
        {
            Configs.Add(_receiveData, new Config(func));
        }

        public void Receive(Func<byte[], byte[]> func)
        {
            Configs.Add(_receiveData, new Config(func));

        }

        public string Match(string request)
        {
            var config = Configs.FirstOrDefault(x => x.Key == request);
            if (config.Equals(new KeyValuePair<string, Config>()))
                config = Configs.FirstOrDefault(x => x.Key == "");
            if (config.Equals(new KeyValuePair<string, Config>())) return "";
            return config.Value.GetResponse(request);
        }
    }
}
