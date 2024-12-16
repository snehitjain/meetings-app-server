namespace meetings_app_server.Models.DTO;

// Request model for removing an attendee
public class RemoveAttendeeRequest
{
    public int MeetingId { get; set; }
    public string UserId { get; set; }
}
