using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rony.Models
{
    public class Config
    {

        public Config(string request, string response)
        {
            Request = request;
            Response = response;
        }
        public Config(string request, Func<string, string> stringFunc)
        {
            Request = request;
            StringFunc = stringFunc;
        }

        public Config(string request, Func<byte[], byte[]> byteFunc)
        {
            Request = request;
            ByteFunc = byteFunc;
        }

        public string Request { get; private set; }
        private string Response { get; set; }
        private Func<string, string> StringFunc { get; set; } = null;
        private Func<byte[], byte[]> ByteFunc { get; set; } = null;

        public string GetResponse(string request)
        {
            if (StringFunc != null) return StringFunc(request);
            if (ByteFunc != null) return ByteFunc(request.GetBytes()).GetString();
            return Response;
        }
    }
}
