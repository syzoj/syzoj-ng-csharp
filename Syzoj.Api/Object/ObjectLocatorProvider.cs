using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Syzoj.Api.Object
{
    public class ObjectLocatorProvider : IObjectLocatorProvider
    {
        private readonly IObjectLocator locatorInstance;
        public ObjectLocatorProvider(IEnumerable<IBaseObjectLocator> locators)
        {
            IDictionary<string, IBaseObjectLocator> dict = new Dictionary<string, IBaseObjectLocator>();
            foreach(var locator in locators)
            {
                dict.Add(locator.Name, locator);
            }
            this.locatorInstance = new ObjectLocator(dict);
        }
        public Task<IObjectLocator> GetDefaultObjectLocator()
        {
            return Task.FromResult<IObjectLocator>(locatorInstance);
        }

        public async Task<IObject> GetObject(Uri uri)
        {
            if(uri == null)
                throw new ArgumentNullException(nameof(uri));
            if(uri.Scheme != "object")
                throw new ArgumentException("URI scheme is not object");
            
            var currentLocator = locatorInstance;
            var segments = uri.Segments.Skip(1);
            foreach(var segment in segments)
            {
                var unescapedSegment = Uri.UnescapeDataString(segment);
                if(unescapedSegment.EndsWith('/'))
                {
                    currentLocator = await currentLocator.GetObjectLocator(unescapedSegment);
                    if(currentLocator == null)
                        return null;
                }
                else
                {
                    return await currentLocator.GetObject(unescapedSegment);
                }
            }
            return currentLocator;
        }

        private class ObjectLocator : IObjectLocator
        {
            private readonly IDictionary<string, IBaseObjectLocator> locators;

            public ObjectLocator(IDictionary<string, IBaseObjectLocator> locators)
            {
                this.locators = locators;
            }

            public Task<IObject> GetObject(string segment) => Task.FromResult<IObject>(null);

            public Task<IObjectLocator> GetObjectLocator(string segment)
            {
                if(!segment.EndsWith('/'))
                    throw new ArgumentException("Segment name does not end with slash");
                IBaseObjectLocator baseLocator;
                if(locators.TryGetValue(segment.Remove(segment.Length - 1), out baseLocator))
                    return baseLocator.GetObjectLocator();
                return Task.FromResult<IObjectLocator>(null);
            }

            public Uri GetUri()
            {
                return new Uri("object:///");
            }
        }
    }
}