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

        public async Task<IObject> GetObject(ApplicationDbContext dbContext, Guid Id)
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

            var obj = await provider.GetObject(dbContext, Id);
            this.logger.LogDebug("Object {id}: {obj}", Id, obj);
            return obj;
        }

        public Task<Guid> CreateObject<TObjectProvider>(ApplicationDbContext dbContext)
            where TObjectProvider : IObjectProvider
        {
            var model = new ObjectDbModel()
            {
                Id = Guid.NewGuid(),
                Type = typeof(TObjectProvider).AssemblyQualifiedName,
                Info = null,
            };
            dbContext.Add(model);
            return Task.FromResult(model.Id);
        }

        public async Task DeleteObject(ApplicationDbContext dbContext, Guid Id)
        {
            var model = await dbContext.FindAsync<ObjectDbModel>(Id);
            if(model == null)
            {
                this.logger.LogWarning("Attempting to delete a nonexistent object {Id}", Id);
                return;
            }

            dbContext.Remove(model);
        }
    }
}