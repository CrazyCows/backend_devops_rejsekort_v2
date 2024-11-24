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

            var userId = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                Console.WriteLine(userId);
                Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                return Unauthorized(new { error = "User is not authenticated" });
            }


            var user = await _userManager.FindByEmailAsync(userId);

            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }
            var existingLocationPair = await _context.LocationPairs
                .Where(lp => lp.UserId == user.Id)
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
                UserId = user.Id,
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

            var userId = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                Console.WriteLine(userId);
                Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                return Unauthorized(new { error = "User is not authenticated" });
            }

            var user = await _userManager.FindByEmailAsync(userId);

            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            var locationPair = await _context.LocationPairs
                .Include(lp => lp.SignInLocation)
                .Include(lp => lp.SignOutLocation)
                .FirstOrDefaultAsync(lp => lp.UserId == user.Id && lp.SignOutLocation == null);

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
            var userId = User.FindFirstValue(ClaimTypes.Email);


            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine(userId);
                return Unauthorized(new { error = "User is not Authenticated" } );
            }

            var user = await _userManager.FindByEmailAsync(userId);

            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            Console.WriteLine("---------------------------------------");
            Console.WriteLine(userId);

            Console.WriteLine("---------------------------------------");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine(user.Id);

            Console.WriteLine("---------------------------------------");
            var LocationPair = await _context.LocationPairs
                .Include(lp => lp.SignInLocation)
                .Include(lp => lp.SignOutLocation)
                .FirstOrDefaultAsync(lp => lp.UserId == user.Id && lp.SignOutLocation == null);

            Console.WriteLine(LocationPair);
            Console.WriteLine("---------------------------------------");
            if (LocationPair == null)
            {
                return BadRequest(new { error = "No Sign-in location found" });
            }

            return Ok(LocationPair);

        }
    }
}
