using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using meetings_app_server.Data;
using meetings_app_server.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using meetings_app_server.Models.DTO;
using System.Text.Json.Serialization;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace meetings_app_server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MeetingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public MeetingsController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }


        // POST: api/Meetings
        [HttpPost]
        public async Task<ActionResult<Meeting>> AddMeeting([FromBody] CreateMeetingRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid meeting data.");
            }

            // Create the meeting
            var meeting = new Meeting
            {
                Name = request.Name,
                Description = request.Description,
                Date = request.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };

            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            // Check if attendee userIds are provided and exist
            if (request.Attendees != null && request.Attendees.Any())
            {
                var attendeesToAdd = new List<Attendee>();

                foreach (var attendeeEmail in request.Attendees)
                {
                    // Check if the user exists
                    var user = await _userManager.FindByEmailAsync(attendeeEmail.Email);
                    if (user == null)
                    {
                        // If user does not exist, return a BadRequest with userId that does not exist
                        return BadRequest($"User with ID {attendeeEmail.Email} does not exist.");
                    }

                    // Add attendee to the list to be added
                    attendeesToAdd.Add(new Attendee
                    {
                        MeetingId = meeting.Id,
                        UserId = user.Id

                    });
                }

                // Add all valid attendees to the meeting
                _context.Attendees.AddRange(attendeesToAdd);
                await _context.SaveChangesAsync();
            }


            //Automatically add the logged -in user as an attendee
            var useri = await _userManager.GetUserAsync(User);
            var userId = useri.Id; // Logged-in user
            var attendee = new Attendee
            {
                MeetingId = meeting.Id,
                UserId = userId
            };

            _context.Attendees.Add(attendee);
            await _context.SaveChangesAsync();

            var meetingDto = _mapper.Map<MeetingDto>(meeting);
            return CreatedAtAction(nameof(GetMeeting), new { id = meeting.Id }, meetingDto);
        }

        // GET: api/Meetings/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MeetingDto>> GetMeeting(int id)
        {

            var userId = await _userManager.GetUserAsync(User);
            Console.WriteLine("User ID from token: " + userId);
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            // Check if the user is an attendee of the specified meeting
            var meeting = await _context.Meetings
                .Include(m => m.Attendees) // Ensure attendees are included in the query
                .FirstOrDefaultAsync(m => m.Id == id && m.Attendees.Any(a => a.UserId == userId.Id));


            //var meeting = await _context.Meetings.FindAsync(id);

            if (meeting == null)
            {
                return NotFound();
            }

            // Map to MeetingDto before returning
            var meetingDto = _mapper.Map<MeetingDto>(meeting);
            return Ok(meetingDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetMeetings(
        [FromQuery] string period = "all",
        [FromQuery] string search = "",
        //[FromQuery] string filterOn = null,
        //[FromQuery] string filterQuery = null,
        //[FromQuery] string sortBy = null,
        //[FromQuery] bool? isAscending = true,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 1000,
        [FromQuery] string _embed = ""
        )
        {
            var userId = await _userManager.GetUserAsync(User);
            //Console.WriteLine("User ID from token: " + userId);

            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }
            var query = _context.Meetings
                    .Include(m => m.Attendees)
                    .ThenInclude(a => a.User)
                    .Where(m => m.Attendees.Any(a => a.UserId == userId.Id)) // Ensure the user is part of the meeting
                    .AsQueryable(); // Ensure Include comes first

            var now = DateTime.Now;

            // Apply period filtering (past, present, future, or all)
            if (period == "past")
                query = query.Where(m => m.Date < now);
            else if (period == "present")
                query = query.Where(m => m.Date.Date == now.Date);
            else if (period == "future")
                query = query.Where(m => m.Date > now);


            // Apply search filtering on the description field
            if (!string.IsNullOrEmpty(search))
                //query = query.Where(m => m.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
                query = query.Where(m => m.Description.ToLower().Contains(search.ToLower()));



            // Pagination: Skip and Take
            var totalMeetings = await query.CountAsync();

            // Use AutoMapper to map the query result to MeetingDto
            var meetingsDto = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => _mapper.Map<MeetingDto>(m)) // Mapping to MeetingDto
                .ToListAsync();

            // Return paginated results with metadata
            var result = new
            {
                TotalItems = totalMeetings,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = meetingsDto
            };

            return Ok(result);
        }

        // DELETE /api/Meetings/DeleteMeeting/{id}
        [HttpDelete("DeleteMeeting/{id}")]
        public async Task<ActionResult> DeleteMeeting(int id)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            // Define the super user ID (you could also use a role check here)
            var superUserId = "9cd6fd73-790b-41eb-ab9c-8ef44647b78e";
            // Find the meeting by ID
            var meeting = await _context.Meetings.FindAsync(id);

            // Check if the meeting exists
            if (meeting == null)
            {
                return NotFound("Meeting not found.");
            }

            // Check if the current user is the super user
            if (userId != superUserId)
            {
                return Forbid("You are not authorized to delete this meeting.");
            }

            // Remove the meeting from the database
            _context.Meetings.Remove(meeting);

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle potential errors
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

            // Return NoContent status indicating successful deletion
            return Ok($"Meeting {id} deleted successfully");
        }

        // GET: api/AllMeetings
        [HttpGet("AllMeetings")]
        public async Task<ActionResult<IEnumerable<MeetingDto>>> GetAllMeetings()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            if (user == null)
            {
                return Unauthorized("User not authenticated.");
            }

            // Define the super user ID (you could also use a role check here)
            var superUserId = "9cd6fd73-790b-41eb-ab9c-8ef44647b78e";

            // Check if the current user is the super user
            if (userId != superUserId)
            {
                return Forbid("You are not authorized to view all meetings.");
            }

            // Retrieve all meetings from the database
            var meetings = await _context.Meetings.ToListAsync();

            // Map to DTOs for better abstraction
            var meetingDtos = _mapper.Map<List<MeetingDto>>(meetings);

            // Return the list of meetings
            return Ok(meetingDtos);
        }
    }
}

