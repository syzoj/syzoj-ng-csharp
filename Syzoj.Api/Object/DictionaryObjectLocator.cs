using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syzoj.Api.Object
{
    public class DictionaryObjectLocator : Dictionary<string, IObjectLocator>, IObjectLocator
    {
        private readonly Uri baseUri;
        public DictionaryObjectLocator(Uri baseUri)
        {
            this.baseUri = baseUri;
        }
        public Task<IObject> GetObject(string segment)
        {
            return Task.FromResult<IObject>(null);
        }

        public Task<IObjectLocator> GetObjectLocator(string segment)
        {
            IObjectLocator locator;
            if(!this.TryGetValue(segment.Remove(segment.Length - 1), out locator))
                return Task.FromResult<IObjectLocator>(null);
            return Task.FromResult<IObjectLocator>(locator);
        }

        public Uri GetUri()
        {
            return baseUri;
        }
    }
}