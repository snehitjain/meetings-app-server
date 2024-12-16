namespace meetings_app_server.Models.DTO;

public class CreateMeetingRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public List<string> Attendees { get; set; }  // List of userIds for attendees
}


