using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Data;
using Syzoj.Api.Filters;
using Syzoj.Api.Models.Data;
using Syzoj.Api.Models.Requests;
using Syzoj.Api.Utils;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using MessagePack;
using Syzoj.Api.Services;
using Syzoj.Api.Models;

namespace Syzoj.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ISessionManager sess;

        public AuthController(ApplicationDbContext dbContext, ISessionManager sess)
        {
            this.dbContext = dbContext;
            this.sess = sess;
        }

        // POST /api/auth/login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if(sess.IsAuthenticated())
            {
                return Ok(new {
                    Status = "Fail",
                    Message = "Already logged in"
                });
            }
            var user = await dbContext.Users.FirstOrDefaultAsync((u) => u.UserName == req.UserName);
            if(user == null)
            {
                return Ok(new {
                    Status = "Fail",
                    Message = "User with specified UserName does not exist"
                });
            }
            if(user.CheckPassword(req.Password))
            {
                await sess.AuthenticateUserAsync(user);
                return Ok(new {
                    Status = "Success",
                });
            }
            else
            {
                return Ok(new {
                    Status = "Fail",
                    Message = "Password incorrect",
                });
            }
        }

        // POST /api/auth/logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            if(sess.IsAuthenticated())
            {
                await sess.UnauthenticateUserAsync();
                return Ok("Logout");
            }
            else
            {
                return Ok("Not logged in");
            }
        }

        // POST /api/auth/register
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest addUser)
        {
            try {
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
            catch (DbUpdateException e)
            {
                if(e.InnerException is Npgsql.PostgresException ne)
                {
                    if(ne.SqlState.Equals("23505"))
                    {
                        switch(ne.ConstraintName)
                        {
                            case "IX_Users_UserName":
                                return Ok(new {
                                    status = "UserNameConflict"
                                });
                            case "IX_Users_Email":
                                return Ok(new {
                                    status = "EmailConflict"
                                });
                        }
                    }
                }
                throw e;
            }
        }
    }
}