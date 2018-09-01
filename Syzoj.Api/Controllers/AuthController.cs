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
using Syzoj.Api.Models.Responses;

namespace Syzoj.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    //[ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ISessionManager sess;

        public AuthController(ApplicationDbContext dbContext, ISessionManager sess)
        {
            this.dbContext = dbContext;
            this.sess = sess;
        }

        /// <summary>
        /// Login with the provided credentials.
        /// </summary>
        /// <response code="200">
        /// A JSON object, containing:
        /// - `Status`: A string with `Fail` for failure, or `Success` for success.
        /// - `Message`: If failed, a message describing failure.
        /// - `Code`: 0 if success, and other codes if failed.
        ///
        /// Possible error codes:
        /// - 2001: User with specified UserName does not exist.
        /// - 2002: Already logged in.
        /// - 2003: Password incorrect.
        /// </response>
        /// <response code="400">
        /// The request is invalid.
        /// </response>

        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(InvalidModelStateResponse), 400)]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(req));
            Console.WriteLine(ModelState.IsValid);
            if(sess.IsAuthenticated())
            {
                return Ok(new {
                    Status = "Fail",
                    Code = 2002,
                    Message = "Already logged in.",
                });
            }
            var user = await dbContext.Users.FirstOrDefaultAsync((u) => u.UserName == req.UserName);
            if(user == null)
            {
                return Ok(new {
                    Status = "Fail",
                    Code = 2001,
                    Message = "User with specified UserName does not exist.",
                });
            }
            if(user.CheckPassword(req.Password))
            {
                await sess.AuthenticateUserAsync(user);
                return Ok(new {
                    Status = "Success",
                    Code = 0,
                });
            }
            else
            {
                return Ok(new {
                    Status = "Fail",
                    Code = 2003,
                    Message = "Password incorrect.",
                });
            }
        }

        /// <summary>
        /// Logout.
        /// </summary>
        /// <response code="200">
        /// A JSON object, containing:
        /// - `Status`: A string with `Fail` for failure, or `Success` for success.
        /// - `Message`: If failed, a message describing failure.
        /// - `Code`: 0 if success, and other codes if failed.
        ///
        /// Possible error codes:
        /// - 2001: Not logged in.
        /// </response>
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Logout()
        {
            if(sess.IsAuthenticated())
            {
                await sess.UnauthenticateUserAsync();
                return Ok(new {
                    Status = "Success",
                    Code = 0,
                });
            }
            else
            {
                return Ok(new {
                    Status = "Fail",
                    Code = 2001,
                    Message = "Not logged in."
                });
            }
        }

        // POST /api/auth/register
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Register([FromBody] RegisterRequest addUser)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(addUser));
            Console.WriteLine(ModelState.IsValid);
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
                    Status = "Success",
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
                                return Conflict(new {
                                    Status = "Fail",
                                    Message = "UserNameConflict"
                                });
                            case "IX_Users_Email":
                                return Conflict(new {
                                    Status = "Fail",
                                    Message = "EmailConflict"
                                });
                        }
                    }
                }
                throw e;
            }
        }
    }
}