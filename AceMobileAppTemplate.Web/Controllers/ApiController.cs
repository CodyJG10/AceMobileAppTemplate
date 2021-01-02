using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AceMobileAppTemplate.Entities;
using AceMobileAppTemplate.Web.Data;
using AceMobileAppTemplate.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace AceMobileAppTemplate.Web.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class ApiController : ControllerBase
    {
        private ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;
        private NotificationService _notifications;
        private IEmailSender _emailSender;


        public ApiController(ApplicationDbContext context,
                             UserManager<IdentityUser> userManager,
                             NotificationService notifications,
                             IEmailSender emailSender) 
        {
            _context = context;
            _userManager = userManager;
            _notifications = notifications;
            _emailSender = emailSender;
        }


        [HttpGet("UserData")]
        public async Task<string> GetUserData(string? id, string? email)
        {
            IdentityUser user;
            if (id != null)
            {
                user = await _userManager.FindByIdAsync(id);
            }
            else
            {
                user = await _userManager.FindByEmailAsync(email);
            }
            return JsonConvert.SerializeObject(user);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] string username, [FromForm] string email, [FromForm] string password, [FromForm] string confirmPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!password.Equals(confirmPassword))
            {
                ModelState.AddModelError("", "Password and Confirm Password do not match");
                return BadRequest(ModelState);
            }

            IdentityUser user = new IdentityUser
            {
                UserName = username,
                Email = email,
            };

            if (_context.Users.Any(x => x.UserName == username))
            {
                ModelState.AddModelError("", "the username " + username + " is already taken.");
                return BadRequest(ModelState);
            }

            IdentityResult result = await _userManager.CreateAsync(user, password);
            var errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (await _userManager.CheckPasswordAsync(user, password))
            {
                return Ok();
            }
            else 
            {
                return BadRequest();
            }
        }

        private IActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return BadRequest(404);
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

                if (ModelState.IsValid)
                {
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        [HttpPost("ForgotPassword")]
        public async void ForgotPassword([FromForm] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return;
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                email,
                "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }
    }
}