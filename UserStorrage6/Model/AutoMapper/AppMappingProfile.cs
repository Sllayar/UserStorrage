using AutoMapper;

using UserStorrage6.Model.DB;
using UserStorrage6.Model.Short;

namespace UserStorrage6.Model.AutoMapper
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<UserShort, User>().ForMember(x => x.Service, opt => opt.Ignore());
            CreateMap<User, UserShort>().ForMember(x => x.ServiceKey, opt => opt.Ignore());

            CreateMap<Role, RoleRequest>().ForMember(x => x.ServiceKey, opt => opt.Ignore());
            CreateMap<RoleRequest, Role>().ForMember(x => x.Service, opt => opt.Ignore());

            CreateMap<Permission, PermissionRequest>().ForMember(x => x.ServiceKey, opt => opt.Ignore());
            CreateMap<PermissionRequest, Permission>().ForMember(x => x.Service, opt => opt.Ignore());

            CreateMap<Service, ServiceRequest>().ReverseMap();
        }
    }
}
