using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Linq;
using System.Text;

namespace Syzoj.Api.Utils
{
    public static class MiscUtils
    {
        private static RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public static byte[] GetRandomBytes(int len)
        {
            var bytes = new byte[len];
            _rng.GetBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// Checks if username consist of only lowercase letters, digits,
        /// underscore and hyphen, and is between 3 and 32 characters.
        /// </summary>
        public static bool CheckUserName(string UserName)
        {
            return UserName.Length >= 3 && UserName.Length <= 32 && UserName.ToCharArray().All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_');
        }

        public static string ConvertToHex(byte[] bytes)
        {
            StringBuilder s = new StringBuilder();
            for(int i = 0; i < bytes.Length; ++i)
            {
                s.Append(bytes[i].ToString("x2"));
            }
            return s.ToString();
        }
    }
}