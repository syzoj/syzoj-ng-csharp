using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Linq;

namespace Syzoj.Api
{
    public static class Utils
    {
        // The piece of code is taken from https://github.com/aspnet/Identity/blob/c7276ce2f76312ddd7fccad6e399da96b9f6fae1/src/Core/PasswordHasher.cs
        // Compares two byte arrays for equality. The method is specifically written so that the loop is not optimized.
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }

        public static string GenerateToken(int digits)
        {
            var rng = new RNGCryptoServiceProvider();
            var data = new byte[digits];
            rng.GetBytes(data);
            return String.Concat(data.Select(b => b.ToString("X2")));
        }
    }
}