using AutoMapper;
using meetings_app_server.Models.Domain;
using meetings_app_server.Models.DTO;

namespace meetings_app_server.Mapping
{
    public class AutoMapperProfiles: Profile
    {
        public  AutoMapperProfiles()
        {
            CreateMap<Meeting, MeetingDto>().ReverseMap();
            //.ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString("HH:mm")))
            //.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString("HH:mm")));

            CreateMap<Attendee, MeetingAttendees>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))  // Map User's Email
                 .ReverseMap();

            CreateMap<Attendee, MeetingAttendees2>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))  // Map User's Email
                 .ReverseMap();

            //CreateMap<AddAttendeeRequest, Attendee>().ReverseMap();


        }
        
    }
}
