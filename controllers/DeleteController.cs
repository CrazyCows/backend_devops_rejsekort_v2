using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using backend_devops_rejsekort_v2.dto;
using backend_devops_rejsekort_v2.dal;

namespace backend_devops_rejsekort_v2.controllers
{
    public class DeleteController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (await _userManager.HasPasswordAsync(user))
            {
                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    return Unauthorized("Invalid password.");
                }
            }
            else
            {
                // For users without a password (e.g., Google sign-in), skip password verification.
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok("User account deleted successfully.");
            }

            return BadRequest(result.Errors);
        }
    }
}
