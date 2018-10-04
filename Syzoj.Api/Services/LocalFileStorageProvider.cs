using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Syzoj.Api.Services
{
    public class LocalFileStorageProvider : IAsyncFileStorageProvider
    {
        // The path should be a fully qualified path that ends with a slash.
        private readonly string path;
        private readonly string secret;
        public LocalFileStorageProvider(string path, string secret)
        {
            var realPath = Path.GetFullPath(path + Path.DirectorySeparatorChar);
            if(!Directory.Exists(realPath))
                throw new IOException("Directory doesn't exist.");
            this.path = realPath;
            this.secret = secret;
        }

        public string GetPath()
        {
            return path;
        }

        public string GetSecret()
        {
            return secret;
        }

        public string GenerateSignature(string data)
        {
            var hm = new HMACSHA256(Encoding.ASCII.GetBytes(secret));
            var byteArray = Encoding.ASCII.GetBytes(data);
            var hash = hm.ComputeHash(byteArray);
            return Convert.ToBase64String(hash);
        }

        public bool VerifySignature(string data, string signature)
        {
            var hm = new HMACSHA256(Encoding.ASCII.GetBytes(secret));
            var byteArray = Encoding.ASCII.GetBytes(data);
            var hash = hm.ComputeHash(byteArray);
            try
            {
                var hash2 = Convert.FromBase64String(signature);
                return Utils.ByteArraysEqual(hash, hash2);
            }
            catch(FormatException)
            {
                return false;
            }
        }

        public Task<string> GenerateDownloadLink(string path, string fileName)
        {
            var realPath = Path.Combine(this.path, path);
            if(!File.Exists(realPath))
                return Task.FromResult<string>(null);
            var expire = ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeMilliseconds() + 60000;
            var expireEncoded = WebUtility.UrlEncode(expire.ToString());
            var pathEncoded = WebUtility.UrlEncode(path);
            var fileNameEncoded = WebUtility.UrlEncode(fileName);
            var str = "download" + "\n" + path + "\n" + expire;
            var signature = GenerateSignature(str);
            var signatureEncoded = WebUtility.UrlEncode(signature);
            var url = $"/file/download?expire={expireEncoded}&filename={fileNameEncoded}&path={pathEncoded}&signature={signatureEncoded}";
            return Task.FromResult(url);
        }

        public Task<string> GenerateUploadLink(string path)
        {
            var expire = ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeMilliseconds() + 60000;
            var expireEncoded = WebUtility.UrlEncode(expire.ToString());
            var pathEncoded = WebUtility.UrlEncode(path);
            var str = "upload" + "\n" + path + "\n" + expire;
            var signature = GenerateSignature(str);
            var signatureEncoded = WebUtility.UrlEncode(signature);
            var url = $"/file/upload?expire={expireEncoded}&path={pathEncoded}&signature={signatureEncoded}";
            return Task.FromResult(url);
        }

        public Task<IEnumerable<string>> GetFiles(string path)
        {
            Console.WriteLine(this.path);
            var realPath = Path.Combine(this.path, path);
            try
            {
                return Task.FromResult(Directory.EnumerateFiles(realPath).Select(s => Path.GetFileName(s)));
            }
            catch(DirectoryNotFoundException)
            {
                return Task.FromResult(Enumerable.Empty<string>());
            }
        }
    }
}