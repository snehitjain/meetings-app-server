//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using meetings_app_server.Data;
//using meetings_app_server.Models.Domain;
//using Microsoft.AspNetCore.Authorization;
//using meetings_app_server.Models.DTO;
//using System.Text.Json.Serialization;
//using System.Text.Json;
//using AutoMapper;
//using Microsoft.AspNetCore.Identity;

//namespace meetings_app_server.Controllers
//{

//    [Route("api/[controller]")]
//    [ApiController]
//    public class MeetingsController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IMapper _mapper;

//        public MeetingsController(ApplicationDbContext context,IMapper mapper)
//        {
//            _context = context;
//            _mapper = mapper;
//        }

//        //// GET: api/Meetings
//        //[HttpGet]
//        //public async Task<ActionResult<IEnumerable<Meeting>>> GetMeetings()
//        //{
//        //    var meeting= await _context.Meetings
//        //        .Include(m => m.Attendees)
//        //        .ThenInclude(a => a.User)
              
//        //       .ToListAsync();
//        //    return Ok(_mapper.Map<List<MeetingDto>>(meeting));
//        //}
        
        

//        // POST: api/Meetings
//        [HttpPost]
//        public async Task<ActionResult<Meeting>> AddMeeting([FromBody] CreateMeetingRequest request)
//        {
//            if (request == null)
//            {
//                return BadRequest("Invalid meeting data.");
//            }

//            // Create the meeting
//            var meeting = new Meeting
//            {
//                Name = request.Name,
//                Description = request.Description,
//                Date = request.Date,
//                StartTime = request.StartTime,
//                EndTime = request.EndTime
//            };

//            _context.Meetings.Add(meeting);
//            await _context.SaveChangesAsync();

//            // Check if attendee userIds are provided and exist
//            if (request.Attendees != null && request.Attendees.Any())
//            {
//                var attendeesToAdd = new List<Attendee>();

//                foreach (var usersId in request.Attendees)
//                {
//                    // Check if the user exists
//                    var user = await _context.Users.FindAsync(usersId);
//                    if (user == null)
//                    {
//                        // If user does not exist, return a BadRequest with userId that does not exist
//                        return BadRequest($"User with ID {usersId} does not exist.");
//                    }

//                    // Add attendee to the list to be added
//                    attendeesToAdd.Add(new Attendee
//                    {
//                        MeetingId = meeting.Id,
//                        UserId = usersId
//                    });
//                }

//                // Add all valid attendees to the meeting
//                _context.Attendees.AddRange(attendeesToAdd);
//                await _context.SaveChangesAsync();
//            }


//            //Automatically add the logged -in user as an attendee
//            var userId = User.Identity.Name; // Logged-in user
//            var attendee = new Attendee
//            {
//                MeetingId = meeting.Id,
//                UserId = userId
//            };

//            _context.Attendees.Add(attendee);
//            await _context.SaveChangesAsync();


//            return CreatedAtAction(nameof(GetMeeting), new { id = meeting.Id }, meeting);
//        }

//        // GET: api/Meetings/{id}
//        [HttpGet("{id}")]
//        public async Task<ActionResult<Meeting>> GetMeeting(int id)
//        {
//            var meeting = await _context.Meetings.FindAsync(id);

//            if (meeting == null)
//            {
//                return NotFound();
//            }

//            return meeting;
//        }


//        //// POST: api/Meetings/{meetingId}/attendees
//        //[HttpPost("{meetingId}/attendees")]
//        //public async Task<ActionResult> AddAttendee(int meetingId, [FromBody] string userId)
//        //{
//        //    var meeting = await _context.Meetings.FindAsync(meetingId);
//        //    if (meeting == null) return NotFound();

//        //    var user = await _context.Users.FindAsync(userId);
//        //    if (user == null) return NotFound();

//        //    var attendee = new Attendee
//        //    {
//        //        MeetingId = meetingId,
//        //        UserId = userId
//        //    };

//        //    _context.Attendees.Add(attendee);
//        //    await _context.SaveChangesAsync();



//        //    return Ok();

//        //}




//        ////GET /api/meetings?period=all&search=xyz
//        //[HttpGet]
//        //public async Task<ActionResult<IEnumerable<Meeting>>> GetMeetingsByPeriodAndSearch(string period = "all", string search = "")
//        //{
//        //    var userId = User.Identity.Name; // Logged-in user
//        //    var query = _context.Meetings
//        //        .Where(m => m.Attendees.Any(a => a.UserId == userId))
//        //        .Include(m => m.Attendees)
//        //        .ThenInclude(a => a.User);



//        //    var meetings = await query.ToListAsync();
//        //    return Ok(meetings);
//        //}

       
        
      
//            //{
//            //    var userId = User.Identity.Name; // Get logged-in user

//            //    // Start building the query
//            //    var query = _context.Meetings
//            //        .Where(m => m.Attendees.Any(a => a.UserId == userId)) // Ensure user is part of the meeting
//            //        .Include(m => m.Attendees)
//            //        .ThenInclude(a => a.User);

//        [HttpGet]
//        public async Task<IActionResult> GetMeetings(
//        [FromQuery] string period = "all",
//        [FromQuery] string search = "",
//        [FromQuery] string filterOn = null,
//        [FromQuery] string filterQuery = null,
//        [FromQuery] string sortBy = null,
//        [FromQuery] bool? isAscending = true,
//        [FromQuery] int pageNumber = 1,
//        [FromQuery] int pageSize = 1000,
//        [FromQuery] string _embed = ""
//        )
//               {
//                var query = _context.Meetings
//                    .Include(m => m.Attendees)
//                    .ThenInclude(a => a.User).AsQueryable(); // Ensure Include comes first

//                var now = DateTime.Now;

//                // Apply period filtering (past, present, future, or all)
//                if (period == "past")
//                    query = query.Where(m => m.Date < now);
//                else if (period == "present")
//                    query = query.Where(m => m.Date.Date == now.Date);
//                else if (period == "future")
//                    query = query.Where(m => m.Date > now);

//                // Apply search filtering on the description field
//                if (!string.IsNullOrEmpty(search))
//                    query = query.Where(m => m.Description.Contains(search, StringComparison.OrdinalIgnoreCase));

//                // Apply dynamic filtering if provided
//                if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
//                {
//                    // Handle filtering on `Date`, `StartTime`, `EndTime`, and `Attendees`
//                    if (filterOn.Equals("date", StringComparison.OrdinalIgnoreCase))
//                    {
//                        if (DateTime.TryParse(filterQuery, out var filterDate))
//                        {
//                            query = query.Where(m => m.Date.Date == filterDate.Date);
//                        }
//                        else
//                        {
//                            return BadRequest("Invalid date format.");
//                        }
//                    }
//                    else if (filterOn.Equals("startTime", StringComparison.OrdinalIgnoreCase))
//                    {
//                        var times = filterQuery.Split(":");
//                        if (times.Length == 2 && int.TryParse(times[0], out var hours) && int.TryParse(times[1], out var minutes))
//                        {
//                            query = query.Where(m => m.StartTime.Hour == hours && m.StartTime.Minute == minutes);
//                        }
//                        else
//                        {
//                            return BadRequest("Invalid start time format. Expected format: HH:mm.");
//                        }
//                    }
//                    else if (filterOn.Equals("endTime", StringComparison.OrdinalIgnoreCase))
//                    {
//                        var times = filterQuery.Split(":");
//                        if (times.Length == 2 && int.TryParse(times[0], out var hours) && int.TryParse(times[1], out var minutes))
//                        {
//                            query = query.Where(m => m.EndTime.Hour == hours && m.EndTime.Minute == minutes);
//                        }
//                        else
//                        {
//                            return BadRequest("Invalid end time format. Expected format: HH:mm.");
//                        }
//                    }
//                    else if (filterOn.Equals("attendeeEmail", StringComparison.OrdinalIgnoreCase))
//                    {
//                        query = query.Where(m => m.Attendees.Any(a => a.User.Email.Contains(filterQuery, StringComparison.OrdinalIgnoreCase)));
//                    }
//                    else
//                    {
//                        return BadRequest($"Invalid filter field: {filterOn}");
//                    }
//                }

//                // Apply sorting if specified
//                if (!string.IsNullOrEmpty(sortBy) && typeof(Meeting).GetProperty(sortBy) != null)
//                {
//                    if (isAscending.HasValue && isAscending.Value)
//                    {
//                        query = query.OrderBy(m => EF.Property<object>(m, sortBy));
//                    }
//                    else
//                    {
//                        query = query.OrderByDescending(m => EF.Property<object>(m, sortBy));
//                    }
//                }
//                //else
//                //{
//                //    return BadRequest("Invalid sort field.");
//                //}

//                // Pagination: Skip and Take
//                var totalMeetings = await query.CountAsync();

//                // Use AutoMapper to map the query result to MeetingDto
//                var meetingsDto = await query
//                    .Skip((pageNumber - 1) * pageSize)
//                    .Take(pageSize)
//                    .Select(m => _mapper.Map<MeetingDto>(m)) // Mapping to MeetingDto
//                    .ToListAsync();

//                // Return paginated results with metadata
//                var result = new
//                {
//                    TotalItems = totalMeetings,
//                    PageNumber = pageNumber,
//                    PageSize = pageSize,
//                    Items = meetingsDto
//                };

//                return Ok(result);
//            }
//        }
//    }

