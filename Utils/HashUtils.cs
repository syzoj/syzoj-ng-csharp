using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Syzoj.Api.Utils
{
    public static class HashUtils
    {
        /// <summary>
        /// Generate a salt and hash the password with this salt.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <returns>A tuple of bytes representing (Salt, HashedPassword).</returns>
        public static (byte[], byte[]) GenerateHashedPassword(string password)
        {
            // Code from https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-2.1

            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hashed = GetHash(salt, password);

            return (salt, hashed);
        }

        private static byte[] GetHash(byte[] salt, string password)
        {
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);
        }

        private static bool ConstantTimeCompare(byte[] a, byte[] b)
        {
            // Code from https://codahale.com/a-lesson-in-timing-attacks/
            if (a.Length != b.Length)
                return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++) {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }

        public static bool VerifyHash(byte[] salt, byte[] hash, string password)
        {
            byte[] newHash = GetHash(salt, password);
            return ConstantTimeCompare(hash, newHash);
        }
    }
}