using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;

namespace Syzoj.Api.Object
{
    public abstract class DbModelObjectProviderBase<TDbModel, TObject> : IObjectProvider<TObject>
        where TDbModel : DbModelBase
        where TObject : DbModelObjectBase<TDbModel>
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly IObjectService ObjectService;

        public DbModelObjectProviderBase(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
            this.ObjectService = serviceProvider.GetRequiredService<IObjectService>();
        }

        protected abstract Task<TObject> GetObjectImpl(ApplicationDbContext dbContext, TDbModel model);

        public virtual async Task<TObject> GetObject(ApplicationDbContext dbContext, Guid Id)
        {
            var model = await dbContext.FindAsync<TDbModel>(Id);
            return await GetObjectImpl(dbContext, model);
        }

        protected virtual async Task<TObject> CreateObject<TObjectProvider>(ApplicationDbContext dbContext, TDbModel model)
            where TObjectProvider : IObjectProvider
        {
            var id = await ObjectService.CreateObject<TObjectProvider>(dbContext);
            model.Id = id;
            dbContext.Add(model);
            return await GetObjectImpl(dbContext, model);
        }

        Task<IObject> IObjectProvider.GetObject(ApplicationDbContext dbContext, Guid Id)
        {
            return GetObject(dbContext, Id).ContinueWith(o => (IObject) o.Result);
        }
    }
}