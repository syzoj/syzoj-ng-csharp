using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Syzoj.Api.Data;
using Syzoj.Api.Filters;

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
            return $"$(userName) Login!";
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
        public string Register()
        {
            return "Register!";
        }

    }
}
