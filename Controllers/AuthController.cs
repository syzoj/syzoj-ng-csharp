using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Syzoj.Api.Data;
using Syzoj.Api.Filters;
using Syzoj.Api.Models;

namespace Syzoj.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IDistributedCache cache;

        public AuthController(ApplicationDbContext dbContext, IDistributedCache cache)
        {
            this.dbContext = dbContext;
            this.cache = cache;
        }

        // POST /api/auth/login
        [HttpPost]
        public string Login(string userName, string password)
        {
            return $"{userName} Login!";
        }

        // POST /api/auth/logout
        [HttpPost]
        public string Logout()
        {
            return "Logout!";
        }

        // POST /api/auth/register
        [RecaptchaValidation]
        [HttpPost]
        public JsonResult Register([FromForm]RegisterApiModel addUser)
        {
            User user = new User
            {
                Name = addUser.Name,
                Email = addUser.Email,
                // TODO: 添加 Hash
                HashedPassword = addUser.Password,
            };
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            return new JsonResult(new {
                status = 0,
                user = new { name = user.Name, email = user.Email },
            });
        }

    }
}
