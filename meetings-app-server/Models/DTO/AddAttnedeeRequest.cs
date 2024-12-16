namespace meetings_app_server.Models.DTO;

// Request model for adding an attendee
public class AddAttendeeRequest
{
    public int MeetingId { get; set; }
    //public string UserId { get;  set; }
    public string Email { get; set; }
}
