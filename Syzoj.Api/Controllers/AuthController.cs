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
using StackExchange.Redis;
using MessagePack;
using Syzoj.Api.Services;

namespace Syzoj.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ISessionManager sessionManager;

        public AuthController(ApplicationDbContext dbContext, ISessionManager sessionManager
        )
        {
            this.dbContext = dbContext;
            this.sessionManager = sessionManager;
        }

        // POST /api/auth/login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync((u) => u.UserName == req.UserName);
            if(user == null) {
                return Ok(new {
                    Status = "Fail",
                    Message = "User with specified UserName does not exist"
                });
            }
            if(user.CheckPassword(req.Password))
            {
                var sess = new Session() {
                    UserId = user.Id,
                };
                string sessionId = await sessionManager.CreateSession(sess);
                return Ok(new {
                    Status = "Success",
                    SessionID = sessionId,
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
        public string Logout()
        {
            return "Logout!";
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