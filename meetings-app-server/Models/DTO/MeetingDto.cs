using Microsoft.AspNetCore.Identity;

namespace meetings_app_server.Models.DTO;


public class MeetingDto
{

    public int Id { get; set; }  // Primary Key
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }


    public ICollection<MeetingAttendees> Attendees { get; set; }
}
public class MeetingAttendees
{


    //public string UserId { get; set; }

    public string Email { get; set; }
}


