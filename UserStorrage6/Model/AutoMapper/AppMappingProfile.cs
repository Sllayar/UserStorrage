using AutoMapper;

using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.GraphQl;
using UserStorrage6.Model.Requests.Rest;
using UserStorrage6.Model.Requests.Short;

namespace UserStorrage6.Model.AutoMapper
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<UserShort, User>()
                .ForMember(x => x.Service, opt => opt.Ignore())
                .ForMember(x => x.Permissions, opt => opt.Ignore())
                .ForMember(x => x.Roles, opt => opt.Ignore());
            CreateMap<User, UserShort>()
                .ForMember(x => x.ServiceKey, opt => opt.Ignore())
                .ForMember(x => x.Permissions, opt => opt.Ignore())
                .ForMember(x => x.Roles, opt => opt.Ignore());

            CreateMap<ServiceSyncRequest, ServiceSyncUserRequest>()
                .ForMember(x => x.Users, opt => opt.Ignore());
            CreateMap<ServiceSyncUserRequest, ServiceSyncRequest>();

            CreateMap<ServiceSyncRequest, ServiceSyncPermissionRequest>()
                .ForMember(x => x.Permissions, opt => opt.Ignore());
            CreateMap<ServiceSyncPermissionRequest, ServiceSyncRequest>();

            CreateMap<ServiceSyncRequest, RoleSyncRequest>()
                .ForMember(x => x.Roles, opt => opt.Ignore());
            CreateMap<RoleSyncRequest, ServiceSyncRequest>();

            CreateMap<ServiceSyncRequest, Service>()
                .ForMember(x => x.Users, opt => opt.Ignore())
                .ForMember(x => x.Id, opt => opt.Ignore());
            CreateMap<Service, ServiceSyncRequest>();


            CreateMap<PermissionShort, Permission>()
                .ForMember(x => x.Users, opt => opt.Ignore())
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Service, opt => opt.Ignore());
            CreateMap<Permission, PermissionShort>();

            CreateMap<RoleShort, Role>()
                .ForMember(x => x.Users, opt => opt.Ignore())
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Permissions, opt => opt.Ignore())
                .ForMember(x => x.Service, opt => opt.Ignore());
            CreateMap<Role, RoleShort>();

            //CreateMap<Role, RoleShort>().ForMember(x => x.SysId, opt => opt.Ignore());
            //CreateMap<RoleShort, Role>().ForMember(x => x.Service, opt => opt.Ignore());

            //CreateMap<Permission, PermissionShort>().ForMember(x => x.SysId, opt => opt.Ignore());
            //CreateMap<PermissionShort, Permission>().ForMember(x => x.Service, opt => opt.Ignore());

            CreateMap<Service, ServiceSyncUserRequest>().ReverseMap();
        }
    }
}
