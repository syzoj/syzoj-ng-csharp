using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syzoj.Api.Data;

namespace Syzoj.Api.Object
{
    public class ObjectService : IObjectService
    {
        private readonly ILogger<ObjectService> logger;
        private readonly IServiceProvider serviceProvider;

        public ObjectService(ILogger<ObjectService> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public async Task<IObject> GetObject(Guid Id)
        {
            using(var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                var model = await dbContext.Set<ObjectDbModel>().FindAsync(Id);
                if(model == null)
                    return null;
                
                Type t = Type.GetType(model.Type);
                if(t == null)
                {
                    this.logger.LogWarning("Object id {Id} has an invalid Type {Type} associated with it.", model.Id, model.Type);
                    return null;
                }
                if(!typeof(IObjectProvider).IsAssignableFrom(t))
                {
                    this.logger.LogError("Object id {Id} has type {Type} that does not implement IObjectProvider.", model.Id, model.Type);
                    return null;
                }

                IObjectProvider provider = (IObjectProvider) serviceProvider.GetService(t);
                if(provider == null)
                {
                    this.logger.LogError("Object id {Id} has a Type {type} that is not registered in services.", model.Type);
                    return null;
                }

                return await provider.GetObject(Id);
            }
        }

        public async Task<Guid> CreateObject<TObjectProvider>()
            where TObjectProvider : IObjectProvider
        {
            var model = new ObjectDbModel()
            {
                Id = Guid.NewGuid(),
                Type = typeof(TObjectProvider).AssemblyQualifiedName,
                Info = null,
            };
            using(var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                dbContext.Add(model);
                await dbContext.SaveChangesAsync();
            }
            return model.Id;
        }

        public async Task DeleteObject(Guid Id)
        {
            using(var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                var model = await dbContext.Set<ObjectDbModel>().FindAsync(Id);
                if(model == null)
                {
                    this.logger.LogWarning("Attempting to delete a nonexistent object {Id}", Id);
                    return;
                }

                dbContext.Remove(model);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}