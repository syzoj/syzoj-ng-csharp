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
        protected readonly IServiceProvider ServiceProvider;

        public DbModelObjectBase(IServiceProvider serviceProvider, ApplicationDbContext dbContext, TDbModel model)
        {
            this.ServiceProvider = serviceProvider;
            this.DbContext = dbContext;
            this.Model = model;
        }

        public Guid Id => Model.Id;
    }
}