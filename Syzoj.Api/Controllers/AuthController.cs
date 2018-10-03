using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Models;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Syzoj.Api.Mvc;

namespace Syzoj.Api.Controllers
{
    [Route("api/auth")]
    public class AuthController : CustomControllerBase
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public class LoginRequest
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public bool IsPersistent { get; set; }
        }
        /// <summary>
        /// Performs login.
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<CustomResponse>> Login([FromBody] LoginRequest request)
        {
            var result = await signInManager.PasswordSignInAsync(request.UserName, request.Password, request.IsPersistent, false);
            if(result.IsNotAllowed)
            {
                return this.StatusCode(403, new CustomResponse<object>() {
                    Success = false,
                    Errors = new ActionError[] { new ActionError() {
                        Message = "Banned",
                    }},
                });
            }
            else if(result.IsLockedOut)
            {
                return this.StatusCode(403, new CustomResponse<object>() {
                    Success = false,
                    Errors = new ActionError[] { new ActionError() {
                        Message = "Lockout",
                    }},
                });
            }
            else if(result.RequiresTwoFactor)
            {
                return this.StatusCode(403, new CustomResponse<object>() {
                    Success = false,
                    Errors = new ActionError[] { new ActionError() {
                        Message = "RequiresTwoFactor",
                    }},
                });
            }
            else if(result.Succeeded)
            {
                return new CustomResponse();
            }
            else
            {
                return this.StatusCode(403, new CustomResponse<object>() {
                    Success = true,
                    Result = "Failed",
                });
            }
        }

        public class RegisterRequest
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }
        /// <summary>
        /// Performs register.
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<CustomResponse>> Register([FromBody] RegisterRequest request)
        {
            var user = new ApplicationUser() {
                UserName = request.UserName,
                Email = request.Email,
            };
            var result = await userManager.CreateAsync(user, request.Password);
            return new CustomResponse()
            {
                Success = result.Succeeded,
                Errors = result.Errors.Select(e => new ActionError() {
                    Message = $"{e.Code}: {e.Description}",
                }),
            };
        }
    }
}