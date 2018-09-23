using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Models;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Syzoj.Api.Controllers
{
    [Route("api/auth")]
    public class AuthController : ControllerBase
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
        public class LoginResponse : BaseResponse
        {
            /// <summary>
            /// A string indicating the result of login. Possible values are:
            /// - Banned: The user is banned.
            /// - Lockout: The user is locked out.
            /// - RequiresTwoFactor: Second factor authentication is required.
            /// - Success: The login succeeded.
            /// </summary>
            public string Result { get; set; }
        }
        /// <summary>
        /// Performs login.
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var result = await signInManager.PasswordSignInAsync(request.UserName, request.Password, request.IsPersistent, false);
            if(result.IsNotAllowed)
            {
                return new LoginResponse()
                {
                    Success = true,
                    Result = "Banned",
                };
            }
            else if(result.IsLockedOut)
            {
                return new LoginResponse()
                {
                    Success = true,
                    Result = "Lockout",
                };
            }
            else if(result.RequiresTwoFactor)
            {
                return new LoginResponse()
                {
                    Success = true,
                    Result = "RequiresTwoFactor",
                };
            }
            else if(result.Succeeded)
            {
                return new LoginResponse()
                {
                    Success = true,
                    Result = "Success",
                };
            }
            else
            {
                return new LoginResponse()
                {
                    Success = true,
                    Result = "Failed",
                };
            }
        }

        public class RegisterRequest
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }
        public class RegisterResponse : BaseResponse
        {
            /// <summary>
            /// Whether the registration succeeded.
            /// </summary>
            public bool RegisterSucceeded { get; set; }

            /// <summary>
            /// The list of errors.
            /// </summary>
            public IEnumerable<IdentityError> Errors { get; set; }
        }
        /// <summary>
        /// Performs register.
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
        {
            var user = new ApplicationUser() {
                UserName = request.UserName,
                Email = request.Email,
            };
            var result = await userManager.CreateAsync(user, request.Password);
            return new RegisterResponse()
            {
                Success = true,
                RegisterSucceeded = result.Succeeded,
                Errors = result.Errors,
            };
        }
    }
}