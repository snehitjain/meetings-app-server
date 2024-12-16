using Microsoft.AspNetCore.Identity;

namespace meetings_app_server.Models.Domain;
    public class Meeting
    {
      
            public int Id { get; set; }  // Primary Key
            public string Name { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public TimeOnly StartTime { get; set; }
            public TimeOnly EndTime { get; set; }

        public ICollection<Attendee> Attendees { get; set; }


}

