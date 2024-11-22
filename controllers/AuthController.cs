using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using backend_devops_rejsekort_v2.services;
using Microsoft.Extensions.Configuration;
using backend_devops_rejsekort_v2.dto;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using backend_devops_rejsekort_v2.dal;

namespace backend_devops_rejsekort_v2.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;

        public AuthController(UserManager<ApplicationUser> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            Console.WriteLine(user);
            var result = await _userManager.CreateAsync(user, model.Password);
            Console.WriteLine(result);

            if (result.Succeeded)
            {
                // Optionally assign roles
                var token = _tokenService.GenerateToken(user);
                return Ok(new { Token = token });
            }
            Console.WriteLine("Eww");
            return BadRequest(result.Errors);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var token = _tokenService.GenerateToken(user);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }
    }    
}
