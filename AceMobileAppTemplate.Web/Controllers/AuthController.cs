using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AceMobileAppTemplate.Web.Data;
using AceMobileAppTemplate.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace AceMobileAppTemplate.Web.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;
        private IdentityService _identityService;
        private IEmailSender _emailSender;

        public AuthController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IdentityService identityService, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _identityService = identityService;
            _emailSender = emailSender;
        }

        [HttpPost("token")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateToken([FromForm]string username, [FromForm]string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                var token = _identityService.GenerateToken(user);
                string tokenText = new JwtSecurityTokenHandler().WriteToken(token);

                var refreshToken = _identityService.GenerateRefreshToken();
                if (!_context.UserClaims.Any(x => x.ClaimType == "RefreshToken" && x.UserId == user.Id))
                {
                    IdentityUserClaim<string> userClaim = new IdentityUserClaim<string>()
                    {
                        ClaimType = "RefreshToken",
                        UserId = user.Id,
                        ClaimValue = refreshToken
                    };
                    _context.UserClaims.Add(userClaim);
                    _context.SaveChanges();
                }
                else
                {
                    var userClaim = _context.UserClaims.Single(x => x.ClaimType == "RefreshToken" && x.UserId == user.Id);
                    userClaim.ClaimValue = refreshToken;
                    _context.SaveChanges();
                }

                string expirationString = token.Claims.Single(x => x.Type == "exp").Value;
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationString));
                return Ok(new
                {
                    token = tokenText,
                    refreshToken,
                    expiration = dateTimeOffset
                });
            }
            ModelState.AddModelError("", "Your email or password did not match any users. Please verify you have entered the right credentials.");
            return Unauthorized(ModelState);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm]string username, [FromForm]string email, [FromForm]string password, [FromForm]string confirmPassword)
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

        [HttpGet("UserData")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<string> GetUserData()
        {
            var claim = User.Claims.ToList().Single(x => x.Type == JwtRegisteredClaimNames.Jti);
            string id = claim.Value; 
            var user = await _userManager.FindByIdAsync(id);
            return JsonConvert.SerializeObject(user);
        }

        [HttpPost("ForgotPassword")]
        public async void ForgotPassword([FromForm]string email)
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

        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken([FromForm]string token, [FromForm]string refreshToken)
        {
            var principal = _identityService.GetPrincipalFromExpiredToken(token);
            var username = principal.Identity.Name;

            var allClaims = principal.Claims.ToList();
            var name = allClaims.First(c => c.Type.Contains("nameidentifier")).Value;
            var user = _context.Users.Single(x => x.UserName == name);

            var userClaim = _context.UserClaims.Single(x => x.UserId == user.Id && x.ClaimType == "RefreshToken");
            var savedRefreshToken = userClaim.ClaimValue;
            if (savedRefreshToken != refreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            var newJwtToken = _identityService.GenerateToken(user);
            var newRefreshToken = _identityService.GenerateRefreshToken();

            userClaim.ClaimValue = newRefreshToken;
            _context.Update(userClaim);
            _context.SaveChanges();

            string tokenText = new JwtSecurityTokenHandler().WriteToken(newJwtToken);

            string expirationString = newJwtToken.Claims.Single(x => x.Type == "exp").Value;
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationString));

            return new ObjectResult(new
            {
                token = tokenText,
                refreshToken = newRefreshToken,
                expiration = dateTimeOffset
            });
        }
    }
}