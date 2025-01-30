using AutoMapper;
using support.Domain;

namespace support.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Users,UserProfile>();
            CreateMap<SystemAdminDb,SystemAdmins>();
        }
    }
}