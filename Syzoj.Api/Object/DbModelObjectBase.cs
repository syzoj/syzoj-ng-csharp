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
    }
}