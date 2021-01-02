using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rony.Models
{
    public class Config
    {

        public Config(string response)
        {
            Response = response;
        }
        public Config(Func<string, string> stringFunc)
        {
            StringFunc = stringFunc;
        }

        public Config(Func<byte[], byte[]> byteFunc)
        {
            ByteFunc = byteFunc;
        }

        private string Response { get; set; }
        private Func<string, string> StringFunc { get; set; } = null;
        private Func<byte[], byte[]> ByteFunc { get; set; } = null;

        public byte[] GetResponse(string request)
        {
            try
            {
                if (StringFunc != null) return StringFunc(request).GetBytes();
                if (ByteFunc != null) return ByteFunc(request.GetBytes());
                return Response.GetBytes();
            }
            catch (Exception)
            {
            }

            return new byte[] { };
        }
    }
}
