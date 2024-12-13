using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using meetings_app_server.Data;
using meetings_app_server.Models.Domain;
using Microsoft.AspNetCore.Authorization;

namespace meetings_app_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MeetingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Meetings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Meeting>>> GetMeetings()
        {
            return await _context.Meetings.Include(m => m.Attendees).ThenInclude(a => a.User).ToListAsync();
        }

        // POST: api/Meetings/{meetingId}/attendees
        [HttpPost("{meetingId}/attendees")]
        public async Task<ActionResult> AddAttendee(int meetingId, [FromBody] string userId)
        {
            var meeting = await _context.Meetings.FindAsync(meetingId);
            if (meeting == null) return NotFound();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var attendee = new Attendee
            {
                MeetingId = meetingId,
                UserId = userId
            };

            _context.Attendees.Add(attendee);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
