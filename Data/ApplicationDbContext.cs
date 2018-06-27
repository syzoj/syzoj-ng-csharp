using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Models;

namespace Syzoj.Api.Data
{
    public class ApplicationDbContext: DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbOptions)
            : base(dbOptions)
        {
        }

        DbSet<User> Users { get; set; }
    }
}