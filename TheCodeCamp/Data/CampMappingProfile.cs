using AutoMapper;
using TheCodeCamp.Models;

namespace TheCodeCamp.Data
{
    public class CampMappingProfile : Profile
    {
        public CampMappingProfile()
        {
            CreateMap<Camp, CampModel>().ReverseMap();
            CreateMap<Talk, TalkModel>().ReverseMap();
            CreateMap<Speaker, SpeakerModel>().ReverseMap();
        }
    }
}