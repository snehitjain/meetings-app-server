using meetings_app_server.Data;
using meetings_app_server.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using meetings_app_server.Models.DTO;
using Microsoft.EntityFrameworkCore;


namespace meetings_app_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AttendeeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //GET /api/users
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<IdentityUser>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }


        // POST: api/Attendee/Add
        [HttpPost("Add")]
        public async Task<IActionResult> AddAttendee([FromBody] AddAttendeeRequest request)
        {
            var meeting = await _context.Meetings
                .Include(m => m.Attendees)
                .FirstOrDefaultAsync(m => m.Id == request.MeetingId);

            if (meeting == null)
            {
                return NotFound("Meeting not found.");
            }

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Check if the user is already an attendee
            var existingAttendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.MeetingId == request.MeetingId && a.UserId == request.UserId);

            if (existingAttendee != null)
            {
                return BadRequest("User is already an attendee.");
            }

            var attendee = new Attendee
            {
                MeetingId = request.MeetingId,
                UserId = request.UserId
            };

            _context.Attendees.Add(attendee);
            await _context.SaveChangesAsync();

            return Ok("Attendee added successfully.");
        }

        // DELETE: api/Attendee/Remove
        [HttpDelete("Remove")]
        public async Task<IActionResult> RemoveAttendee([FromBody] RemoveAttendeeRequest request)
        {
            var attendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.MeetingId == request.MeetingId && a.UserId == request.UserId);

            if (attendee == null)
            {
                return NotFound("Attendee not found.");
            }

            _context.Attendees.Remove(attendee);
            await _context.SaveChangesAsync();

            return Ok("Attendee removed successfully.");
        }
    }

    // Request model for adding an attendee
    public class AddAttendeeRequest
    {
        public int MeetingId { get; set; }
        public string UserId { get; set; }


    }

    // Request model for removing an attendee
    public class RemoveAttendeeRequest
    {
        public int MeetingId { get; set; }
        public string UserId { get; set; }
    }
}

