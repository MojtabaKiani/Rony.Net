using System.Text;

namespace Rony
{
    public static class Convertor
    {
        public static string GetString(this byte[] input) => Encoding.UTF8.GetString(input);

        public static byte[] GetBytes(this string input) => Encoding.UTF8.GetBytes(input);
    }
}
