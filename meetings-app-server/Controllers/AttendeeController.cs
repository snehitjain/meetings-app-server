using meetings_app_server.Data;
using meetings_app_server.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using meetings_app_server.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace meetings_app_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AttendeeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserWithMeetingsDto>>> GetUsers()
        {
            // Retrieve the users with their associated meeting IDs
            var users = await _context.Users
                .Select(user => new UserWithMeetingsDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Meetings = _context.Attendees
                                    .Where(a => a.UserId == user.Id)
                                    .Select(a => new AttendeesMeeting { MeetingId = a.MeetingId })
                                    .ToList()

                })
                .ToListAsync();

            return Ok(users);
        }

        // POST: api/Attendee/Add
        [HttpPost("Add")]
        public async Task<IActionResult> AddAttendee([FromBody] AddAttendeeRequest request)
        {
            // Get the current logged-in user's ID from the JWT token
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            // Check if the user is authenticated
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            // Get the meeting from the database
            var meeting = await _context.Meetings
                .Include(m => m.Attendees)
                .FirstOrDefaultAsync(m => m.Id == request.MeetingId);

            if (meeting == null)
            {
                return NotFound("Meeting not found.");
            }

            // Check if the logged-in user is already an attendee of the meeting
            var isUserAttendee = meeting.Attendees.Any(a => a.UserId == userId);
            if (!isUserAttendee)
            {
                return Forbid("You must be part of the meeting to add attendees.");
            }

            // Find the user to be added by email (use Email instead of UserId)
            var userToAdd = await _userManager.FindByEmailAsync(request.Email);
            if (userToAdd == null)
            {
                return NotFound("User not found.");
            }

            // Check if the user is already an attendee of the meeting
            var existingAttendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.MeetingId == request.MeetingId && a.UserId == userToAdd.Id);

            if (existingAttendee != null)
            {
                return BadRequest("User is already an attendee.");
            }

            // Add the new attendee using the UserId extracted from userToAdd
            var newAttendee = new Attendee
            {
                MeetingId = request.MeetingId,
                UserId = userToAdd.Id
            };

            _context.Attendees.Add(newAttendee);
            await _context.SaveChangesAsync();

            return Ok("Attendee added successfully.");
        }
        
        [HttpDelete("Remove")]
        public async Task<IActionResult> RemoveAttendee([FromBody] RemoveAttendeeRequest request)
        {
            // Get the current logged-in user's ID from the JWT token
            var user = await _userManager.GetUserAsync(User); 
            var userId = user.Id; 

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            // Find the attendee record in the specified meeting for the logged-in user
            var attendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.MeetingId == request.MeetingId && a.UserId == userId);

            if (attendee == null)
            {
                return NotFound("Attendee not found in this meeting.");
            }

            // Remove the attendee from the meeting
            _context.Attendees.Remove(attendee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

