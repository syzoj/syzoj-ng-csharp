using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Data;
using Syzoj.Api.Filters;
using Syzoj.Api.Models;
using Syzoj.Api.Models.Requests;
using Syzoj.Api.Utils;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync((u) => u.UserName == req.UserName);
            if(user.CheckPassword(req.Password))
            {
                return Ok("pass");
            }
            else
            {
                return Unauthorized();
            }
        }

        // POST /api/auth/logout
        [HttpPost]
        public string Logout()
        {
            return "Logout!";
        }

        // POST /api/auth/register
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest addUser)
        {
            var (salt, pass) = HashUtils.GenerateHashedPassword(addUser.Password);

            var user = new User() {
                UserName = addUser.UserName,
                Email = addUser.Email,
            };
            user.SetPassword(addUser.Password);
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return Ok(new {
                status = "success",
            });
        }
    }
}