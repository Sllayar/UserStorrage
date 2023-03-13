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

        public async Task<List<Role>?> SynhronizePart(
            IDataBrocker dataBrocker, RoleSyncRequest service, DateTime currentdate)
        {
            var currentDate = DateTime.UtcNow;

            var updatedService = await _service.CreateOrUpdate(
                dataBrocker, _mapper.Map<ServiceSyncRequest>(service));

            CheckForExist(dataBrocker.GetPermissions(updatedService.Key).ToList(), service.Roles);

            var existRole = dataBrocker
                .GetRoleFull(updatedService.Id)
                .ToList();

            foreach (var newRole in service.Roles)
            {
                var role = existRole.FirstOrDefault(r => r.SysId == newRole.SysId);

                var existPermissions = dataBrocker
                    .GetPermissionsFull(updatedService.Id)
                    .ToList()
                    .Where(p => !string.IsNullOrEmpty(newRole.Permissions.Find(n => n.Equals(p.SysId))))
                    .ToList();

                if (role == null) AddNewRole(dataBrocker, updatedService, newRole, existPermissions, currentDate, currentdate);
                else TryUpdateRole(role, newRole, updatedService, existPermissions, currentDate, currentdate);
            }

            await dataBrocker.SaveChangesAsync();

            return dataBrocker
                .GetRoleFull(updatedService.Id)
                .ToList()
                .Where(db => service.Roles.FirstOrDefault(s => s.SysId == db.SysId) != null)
                .ToList();
        }

        public async Task<List<Role>?> SynhronizePartFinish(
            IDataBrocker dataBrocker, string serviceKey, DateTime syncDate)
        {
            var roles = dataBrocker.GetRole(serviceKey).ToList();

            foreach (var role in roles)
            {
                if (role.PartSyncAt != syncDate.ToUniversalTime() && 
                    (role != null || role.Status != Status.Delete))
                {
                    role.Status = Status.Delete;
                    role.SyncAt = syncDate.ToUniversalTime();
                    role.UpdateAt = syncDate.ToUniversalTime();
                }
            }

            await dataBrocker.SaveChangesAsync();

            return dataBrocker.GetRole(serviceKey).ToList();
        }

        public async Task<List<Role>?> Synhronize(IDataBrocker dataBrocker, RoleSyncRequest roleSyncRequest, DateTime syncDate)
        {
            var currentDate = DateTime.UtcNow;

            var updatedService = await _service.CreateOrUpdate(
                dataBrocker, _mapper.Map<ServiceSyncRequest>(roleSyncRequest));

            CheckForExist(dataBrocker.GetPermissions(updatedService.Key).ToList(), roleSyncRequest.Roles);

            var existRole = dataBrocker
                .GetRoleFull(updatedService.Id)
                .ToList();

            foreach (var newRole in roleSyncRequest.Roles)
            {
                var role = existRole.FirstOrDefault(r => r.SysId == newRole.SysId);

                var existPermissions = dataBrocker
                    .GetPermissionsFull(updatedService.Id)
                    .ToList()
                    .Where(p => !string.IsNullOrEmpty(newRole.Permissions.Find(n => n.Equals(p.SysId))))
                    .ToList();

                if (role == null) AddNewRole(dataBrocker, updatedService, newRole, existPermissions, currentDate, syncDate);
                else TryUpdateRole(role, newRole, updatedService, existPermissions, currentDate, syncDate);
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

        private void CheckForExist(List<Permission> permissions, List<RoleShort> roleShorts)
        {
            foreach (var role in roleShorts)
                if(role.Permissions.Except(permissions.Select(p => p.SysId)).Count() > 0)
                    throw new Exception($"Обнаружена попытка добавить несуществующее право.");
        }

        private Role TryUpdateRole(Role role, RoleShort roleShort,
            Service service, List<Permission>? permissions, DateTime currentDate, DateTime syncTime)
        {
            role.SyncAt = currentDate.ToUniversalTime();
            role.PartSyncAt = syncTime.ToUniversalTime();

            if (role.Name == roleShort.Name &&
                role.Description == roleShort.Description &&
                role.Status == roleShort.Status &&
                role.IsNeedAprove == roleShort.IsNeedAprove &&
                role.Comment == roleShort.Comment)
                return role;

            role.Service = service;

            role.Permissions = permissions;

            role.UpdateAt = syncTime.ToUniversalTime();

            role.Name = roleShort.Name;
            role.Description = roleShort.Description;
            role.Status = roleShort.Status;
            role.IsNeedAprove = roleShort.IsNeedAprove;
            role.Comment = roleShort.Comment;

            role.Users?.ForEach(u =>
                u.UpdateAt = currentDate.ToUniversalTime());

            return role;
        }

        private Role AddNewRole(IDataBrocker dataBrocker, Service serviceKey, RoleShort roleShort,
            List<Permission>? permissions, DateTime currentdate, DateTime syncTime)
        {
            var role = _mapper.Map<Role>(roleShort);

            role.Service = serviceKey;
            role.Permissions = permissions;

            role.UpdateAt = syncTime.ToUniversalTime();
            role.CreateAT = currentdate.ToUniversalTime();
            role.SyncAt = currentdate.ToUniversalTime();
            role.PartSyncAt = syncTime.ToUniversalTime();

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
                    curRoles.SyncAt = currentdate.ToUniversalTime();
                    curRoles.UpdateAt = syncTime.ToUniversalTime();
                }
            }
        }


    }
}
