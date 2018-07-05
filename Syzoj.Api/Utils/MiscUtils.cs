using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;

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
    }
}