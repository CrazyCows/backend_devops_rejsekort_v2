using backend_devops_rejsekort_v2.dal;
using backend_devops_rejsekort_v2.dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend_devops_rejsekort_v2.controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize] // Ensure the user is authenticated
    public class LocationController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LocationController(UserContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> SignInOnLocation([FromBody] Location location)
        {
            if (location == null)
            {
                return BadRequest(new { error = "No location is given" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User is not authenticated" });
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            var existingLocationPair = await _context.LocationPairs
                .Where(lp => lp.UserId == userId)
                .OrderByDescending(lp => lp.SignInTime)
                .Include(lp => lp.SignInLocation)
                .Include(lp => lp.SignOutLocation)
                .FirstOrDefaultAsync();

            if (existingLocationPair != null && existingLocationPair.SignOutLocation == null)
            {
                return BadRequest(new { error = "User has not signed out of the previous location" });
            }

            var locationPair = new LocationPair
            {
                SignInLocation = location,
                SignInTime = DateTime.UtcNow,
                UserId = userId,
                User = user
            };

            await _context.LocationPairs.AddAsync(locationPair);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User sign-in location has been saved" });
        }

        [HttpPost]
        public async Task<IActionResult> SignOutOnLocation([FromBody] Location location)
        {
            if (location == null)
            {
                return BadRequest(new { error = "No location is given" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User is not authenticated" });
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            var locationPair = await _context.LocationPairs
                .Include(lp => lp.SignInLocation)
                .Include(lp => lp.SignOutLocation)
                .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.SignOutLocation == null);

            if (locationPair == null)
            {
                return BadRequest(new { error = "No sign-in location found" });
            }

            locationPair.SignOutLocation = location;
            locationPair.SignOutTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "User sign-out location has been saved" });
        }
    [HttpPost]
    public async Task<IActionResult> LocationSignedIn()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) {
                return Unauthorized(new { error = "User is not Authenticated" } );
            }

            var LocationPair = await _context.LocationPairs
                .Include(lp => lp.SignInLocation)
                .Include(lp => lp.SignOutLocation)
                .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.SignOutLocation == null);

            if (LocationPair == null)
            {
                return BadRequest(new { error = "No Sign-in location found" });
            }

            return Ok(LocationPair);

        }
    }
}
