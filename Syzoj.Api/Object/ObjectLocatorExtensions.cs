using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Syzoj.Api.Object
{
    public static class ObjectLocatorExtensions
    {
        public static IServiceCollection AddBaseObjectLocator<T>(this IServiceCollection collection)
            where T : class, IBaseObjectLocator
            => collection.AddSingleton<IBaseObjectLocator, T>();
        
        public static IServiceCollection AddObjectLocatorProvider(this IServiceCollection collection)
            => collection.AddSingleton<IObjectLocatorProvider, ObjectLocatorProvider>();
        
        public static IServiceCollection AddBaseObjectLocator<T>(this IServiceCollection collection, string name)
            where T : class, IObjectLocator
            => collection.AddSingleton<T>()
                .AddSingleton<IBaseObjectLocator>((serviceProvider) => new BaseObjectLocator(name, serviceProvider.GetRequiredService<T>()));

        private class BaseObjectLocator : IBaseObjectLocator
        {
            public BaseObjectLocator(string name, IObjectLocator locator)
            {
                this.Name = name;
                this.locator = locator;
            }

            public string Name { get; }
            private readonly IObjectLocator locator;

            public Task<IObjectLocator> GetObjectLocator()
            {
                return Task.FromResult<IObjectLocator>(this.locator);
            }
        }
    }
}