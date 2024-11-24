using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using backend_devops_rejsekort_v2.services;
using Microsoft.Extensions.Configuration;
using backend_devops_rejsekort_v2.dto;
using backend_devops_rejsekort_v2.dal;

namespace backend_devops_rejsekort_v2.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleAuthController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;

        public GoogleAuthController(UserManager<ApplicationUser> userManager, TokenService tokenService, IConfiguration configuration)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginModel model)
        {
            var payload = await VerifyGoogleToken(model.IdToken);
            if (payload == null)
            {
                return Unauthorized();
            }

            var info = new UserLoginInfo("Google", payload.Subject, "Google");
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                user = new ApplicationUser { Email = payload.Email, UserName = payload.Email };
                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                await _userManager.AddLoginAsync(user, info);
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(new { Token = token });
        }

        private async Task<Google.Apis.Auth.GoogleJsonWebSignature.Payload> VerifyGoogleToken(string idToken)
        {
            try
            {
                var settings = new Google.Apis.Auth.GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { _configuration["Authentication:Google:ClientId"] }
                };

                var payload = await Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                return payload;
            }
            catch
            {
                return null;
            }
        }

    }
    

}
