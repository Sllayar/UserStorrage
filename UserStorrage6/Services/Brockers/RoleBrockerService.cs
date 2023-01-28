using AutoMapper;
using UsersStorrage.Models.Context;

using UserStorrage6.Model.Context.Repository;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.GraphQl;
using UserStorrage6.Model.Requests.Rest;
using UserStorrage6.Model.Requests.Short;
using UserStorrage6.Services.Interfaces;

namespace UserStorrage6.Services.Brockers
{
    public class RoleBrockerService : IRoleBrockerService
    {
        private readonly IPermissionBrockerService _permissionService;
        private readonly IServicesBrockersService _service;
        private readonly IMapper _mapper;

        public RoleBrockerService(
            IMapper mapper,
            IServicesBrockersService service,
            IPermissionBrockerService permissionService)
        {
            _mapper = mapper;
            _service = service;
            _permissionService = permissionService;
        }

        public async Task<List<Role>?> Synhronize(IDataBrocker dataBrocker, RoleSyncRequest roleSyncRequest, DateTime syncDate)
        {
            var currentDate = DateTime.UtcNow;

            var updatedService = await _service.CreateOrUpdate(
                dataBrocker, _mapper.Map<ServiceSyncRequest>(roleSyncRequest));

            var existRole = dataBrocker
                .GetRoleFull(updatedService.Id)
                .ToList();

            foreach (var newRole in roleSyncRequest.Roles)
            {
                var role = existRole.FirstOrDefault(r => r.SysId == newRole.SysId);

                var permission = await _permissionService
                    .Synhronize(dataBrocker, newRole.Permitions, updatedService, currentDate, syncDate);

                if (role == null) AddNewRole(dataBrocker, updatedService, newRole, permission, currentDate, syncDate);
                else TryUpdateRole(role, newRole, updatedService, permission, currentDate, syncDate);
            }


            var exceptPermissions = existRole?
                .Select(s => s.SysId)
                .Except(roleSyncRequest.Roles.Select(s => s.SysId))
                .ToList();

            DeleteNotUpdatedRoles(existRole, exceptPermissions, currentDate, syncDate);

            await dataBrocker.SaveChangesAsync();

            return dataBrocker
                .GetRoleFull(updatedService.Id)
                .ToList();
        }

        private Role TryUpdateRole(Role role, RoleShort roleShort,
            Service service, List<Permission>? permissions, DateTime currentDate, DateTime syncTime)
        {
            role.SyncAt = syncTime.ToUniversalTime();

            if (role.Name == roleShort.Name &&
                role.Description == roleShort.Description &&
                role.Status == roleShort.Status &&
                role.IsNeedAprove == roleShort.IsNeedAprove &&
                role.Comment == roleShort.Comment &&

                (permissions == null ||
                permissions.FirstOrDefault(p =>
                    p.UpdateAt.ToUniversalTime() == currentDate.ToUniversalTime()) == null))
                return role;

            role.Service = service;

            role.Name = roleShort.Name;
            role.Description = roleShort.Description;
            role.Status = roleShort.Status;
            role.IsNeedAprove = roleShort.IsNeedAprove;
            role.Comment = roleShort.Comment;

            role.UpdateAt = currentDate.ToUniversalTime();

            role.Users?.ForEach(u =>
                u.UpdateAt = currentDate.ToUniversalTime());

            return role;
        }

        private Role AddNewRole(IDataBrocker dataBrocker, Service serviceKey, RoleShort roleShort,
            List<Permission>? permissions, DateTime currentdate, DateTime syncTime)
        {
            var role = _mapper.Map<Role>(roleShort);

            role.Service = serviceKey;
            role.SysPermitions = permissions;

            role.UpdateAt = currentdate.ToUniversalTime();
            role.CreateAT = currentdate.ToUniversalTime();
            role.SyncAt = syncTime.ToUniversalTime();

            dataBrocker.AttachRole(role);

            return role;
        }

        private void DeleteNotUpdatedRoles(List<Role>? roles,
            List<string>? exceptRoles, DateTime currentdate, DateTime syncTime)
        {
            if (roles == null) return;

            foreach (var r in exceptRoles)
            {
                var curRoles = roles
                    .FirstOrDefault(rs => rs.SysId == r);

                if (curRoles != null || curRoles.Status != Status.Delete)
                {
                    curRoles.Status = Status.Delete;
                    curRoles.SyncAt = syncTime.ToUniversalTime();
                    curRoles.UpdateAt = currentdate.ToUniversalTime();
                }
            }
        }
    }
}
