using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public AuthController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
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
        public async Task<JsonResult> Register([FromForm]RegisterApiModel addUser)
        {
            var (salt, hash) = Utils.HashUtils.GenerateHashedPassword(addUser.Password);
            User user = new User
            {
                Name = addUser.Name,
                Email = addUser.Email,
                HashedPassword = Convert.ToBase64String(hash),
                PasswordSalt = Convert.ToBase64String(salt)
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return new JsonResult(new {
                status = 0,
                user = new { name = user.Name, email = user.Email },
            });
        }

    }
}
