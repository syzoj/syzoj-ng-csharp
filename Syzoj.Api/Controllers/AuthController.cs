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
        public string Login([FromBody] LoginRequest req)
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
        [HttpPost]
        public string Register(RegisterApiModel addUser)
        {
            return "Register";
        }
    }
}