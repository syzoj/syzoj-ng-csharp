using Microsoft.Extensions.DependencyInjection;

namespace Syzoj.Api.Object
{
    public static class ObjectLocatorExtensions
    {
        public static IServiceCollection AddBaseObjectLocator<T>(this IServiceCollection collection)
            where T : class, IBaseObjectLocator
                => collection.AddTransient<T>();
        
        public static IServiceCollection AddObjectLocatorProvider(this IServiceCollection collection)
            => collection.AddSingleton<IObjectLocatorProvider, ObjectLocatorProvider>();
    }
}