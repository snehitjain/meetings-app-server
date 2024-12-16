using AutoMapper;
using meetings_app_server.Models.Domain;
using meetings_app_server.Models.DTO;

namespace meetings_app_server.Mapping
{
    public class AutoMapperProfiles: Profile
    {
        public  AutoMapperProfiles()
        {
            CreateMap<Meeting, MeetingDto>();
            //.ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString("HH:mm")))
            //.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString("HH:mm")));

            CreateMap<Attendee, MeetingAttendees>();
           

        }
        
    }
}
