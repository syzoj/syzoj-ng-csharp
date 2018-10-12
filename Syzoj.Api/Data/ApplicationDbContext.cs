using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Models;

namespace Syzoj.Api.Data
{
    public class ApplicationDbContext : IdentityUserContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            var models = assembly.DefinedTypes
                .Where(ti => ti.GetCustomAttribute(typeof(DbModelAttribute)) != null);
            foreach(var model in models)
            {
                modelBuilder.Entity(model);
                var method = model.GetMethod("OnModelCreating", BindingFlags.Public | BindingFlags.Static);
                if(method != null)
                {
                    method.Invoke(null, new object[] { this, modelBuilder });
                }
            }
        }
    }
}