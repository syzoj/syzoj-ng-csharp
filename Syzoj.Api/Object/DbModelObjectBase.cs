using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;

namespace Syzoj.Api.Object
{
    public abstract class DbModelObjectBase<TDbModel> : IObject
        where TDbModel : DbModelBase
    {
        protected readonly ApplicationDbContext DbContext;
        protected readonly TDbModel Model;

        public DbModelObjectBase(ApplicationDbContext dbContext, TDbModel model)
        {
            this.DbContext = dbContext;
            this.Model = model;
        }

        public Guid Id => Model.Id;

        public abstract class Provider<TObject, TObjectProvider> : IObjectProvider<TObject>
            where TObject : DbModelObjectBase<TDbModel>
            where TObjectProvider : Provider<TObject, TObjectProvider>
        {
            protected readonly IServiceProvider ServiceProvider;
            protected readonly IObjectService ObjectService;

            public Provider(IServiceProvider serviceProvider)
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

            protected virtual async Task<TObject> CreateObject(ApplicationDbContext dbContext, TDbModel model)
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
}