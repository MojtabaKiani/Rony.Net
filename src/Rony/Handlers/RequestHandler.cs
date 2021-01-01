using Rony.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rony.Handlers
{
    public class RequestHandler
    {
        private string _receiveData;
        public List<Config> Configs { get; set; }

        public RequestHandler()
        {
            Configs = new List<Config>();
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
            Configs.Add(new Config(_receiveData, response));
        }

        public void Receive(byte[] response)
        {
            Configs.Add(new Config(_receiveData, response.GetString()));
        }

        public void Receive(Func<string, string> func)
        {
            Configs.Add(new Config(_receiveData, func));
        }

        public void Receive(Func<byte[], byte[]> func)
        {
            Configs.Add(new Config(_receiveData, func));

        }

        public string Match(string request)
        {
            var config = Configs.FirstOrDefault(x => x.Request == request);
            if (config == null)
                config = Configs.FirstOrDefault(x => x.Request == "");
            if (config == null) return "";
            return config.GetResponse(request);
        }
    }
}
