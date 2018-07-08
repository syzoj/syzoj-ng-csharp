using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

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
        public async Task<JsonResult> Register(RegisterApiModel addUser)
        {
            var user = new ApplicationUser()
            {
                RegisteredOn = DateTime.Now,
                UserName = addUser.Name,
                Email = addUser.Email
            };

            await _userManager.CreateAsync(user);
            return new JsonResult(new
            {
                status = 0,
                user = new {id = user.Id},
            });
        }
    }
}