using System;
using System.Threading.Tasks;
using Syzoj.Api.Object;
using Xunit;

namespace Syzoj.Api.Test
{
    public class ObjectLocatorTests
    {
        private class MyObject : IObject
        {
            public Uri GetUri()
            {
                return new Uri("object:///te%2Fst/exi%20sts");
            }
        }
        private class TestBaseObjectLocator : IBaseObjectLocator
        {
            public string Name => "te/st";
            private readonly ObjectLocator locatorInstance;

            public TestBaseObjectLocator()
            {
                locatorInstance = new ObjectLocator();
            }

            public Task<IObjectLocator> GetObjectLocator()
            {
                return Task.FromResult<IObjectLocator>(locatorInstance);
            }

            private class ObjectLocator : IObjectLocator
            {
                public Task<IObject> GetObject(string segment)
                {
                    if(segment == "exi sts")
                    {
                        return Task.FromResult<IObject>(new MyObject());
                    }
                    else
                    {
                        return Task.FromResult<IObject>(null);
                    }
                }

                public Task<IObjectLocator> GetObjectLocator(string segment)
                {
                    if(segment == "recursive/")
                    {
                        return Task.FromResult<IObjectLocator>(this);
                    }
                    else
                    {
                        return Task.FromResult<IObjectLocator>(null);
                    }
                }

                public Uri GetUri()
                {
                    throw new NotImplementedException();
                }
            }
        }
        [Theory]
        // 
        [InlineData("object:///te%2Fst/exi%20sts")]
        [InlineData("object:///te%2Fst/recursive/exi%20sts")]
        public async Task ObjectLocator_AddLocator_GetObject(string path)
        {
            var locators = new IBaseObjectLocator[] {
                new TestBaseObjectLocator()
            };
            var objectLocator = new ObjectLocatorProvider(locators);

            Assert.IsType<MyObject>(await objectLocator.GetObject(new Uri(path)));
        }
        [Theory]
        // An object under a nonexistent folder
        [InlineData("object:///nonexistent/test")]
        // An object under an object
        [InlineData("object:///te%2Fst/exi%20sts/nonexistent")]
        // An object under a nonexistent object
        [InlineData("object:///te%2Fst/recursive/nonexistent")]
        // + is not unescaped
        [InlineData("object:///te%2Fst//exi+sts")]
        // Extra slash is acceptable but usually nonexistent
        [InlineData("object:////te%2Fst//exi%20sts")]
        public async Task ObjectLocator_AddLocator_NonExists(string path)
        {
            var locators = new IBaseObjectLocator[] {
                new TestBaseObjectLocator()
            };
            var objectLocator = new ObjectLocatorProvider(locators);

            Assert.Null(await objectLocator.GetObject(new Uri(path)));
        }
    }
}