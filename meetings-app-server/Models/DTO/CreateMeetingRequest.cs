namespace meetings_app_server.Models.DTO;

public class CreateMeetingRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public ICollection<MeetingAttendees2> Attendees { get; set; }
}
public class MeetingAttendees2
{

    public string Email { get; set; }
    //public string UserId { get; set; }

}
