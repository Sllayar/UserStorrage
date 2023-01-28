using AutoMapper;

using Microsoft.EntityFrameworkCore;

using UsersStorrage.Models.Context;

using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.GraphQl;
using UserStorrage6.Model.Requests.Rest;
using UserStorrage6.Model.Requests.Short;
using UserStorrage6.Services.Interfaces;

namespace UserStorrage6.Services
{
    public class RoleService2 : IRoleService
    {
        private readonly IPermissionService _permissionService;
        private readonly IServicesService _service;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public RoleService2(
            ApplicationDbContext applicationDbContext,
            IMapper mapper,
            IServicesService service,
            IPermissionService permissionService)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _service = service;
            _permissionService = permissionService;
        }

        public async Task Synhronize(RoleSyncRequest roleSyncRequest, Service updatedService,
            DateTime currentDate, DateTime syncDate)
        {
            var existRole = _applicationDbContext.Roles
                .Include(r => r.Users)
                .Include(r => r.SysPermitions)
                .Include(r => r.Service)
                .Where(r => r.Service.Id == updatedService.Id)
                .ToList();

            foreach (var newRole in roleSyncRequest.Roles)
            {
                var role = existRole.FirstOrDefault(r => r.SysId == newRole.SysId);

                if (role == null) AddNewRole(updatedService, newRole, currentDate, syncDate);
                else TryUpdateRole(role, newRole, currentDate, syncDate);
            }

            var exceptPermissions = existRole?
                .Select(s => s.SysId)
                .Except(roleSyncRequest.Roles.Select(s => s.SysId))
                .ToList();

            DeleteNotUpdatedRoles(existRole, exceptPermissions, currentDate, syncDate);

            await _applicationDbContext.SaveChangesAsync();
        }



        public async Task<List<Role>?> Synhronize(RoleSyncRequest roleSyncRequest, DateTime syncDate)
        {
            var currentDate = DateTime.UtcNow;

            var updatedService = await _service.CreateOrUpdate(_mapper.Map<ServiceSyncRequest>(roleSyncRequest));

            await Synhronize(roleSyncRequest, updatedService, currentDate, syncDate);
            await _permissionService.Synhronize(roleSyncRequest, currentDate, syncDate);

            var existR = _applicationDbContext.Roles
                .Where(r => r.Service.Id == updatedService.Id)
                .ToList();

            var existP = _applicationDbContext.Permissions
                .Where(r => r.Service.Id == updatedService.Id)
                .ToList();

            foreach (var role in roleSyncRequest.Roles)
            {
                var er = existR.FirstOrDefault(r => r.SysId == role.SysId);

                foreach (var permition in role.Permitions)
                {
                    er.SysPermitions.Add(existP.FirstOrDefault(p => p.SysId == permition.SysId));
                }
            }

            await _applicationDbContext.SaveChangesAsync();

            return _applicationDbContext.Roles
                .Include(r => r.Users)
                .Include(r => r.SysPermitions)
                .Include(r => r.Service)
                .Where(r => r.Service.Id == updatedService.Id)
                .ToList();
        }

        private Role TryUpdateRole(Role role, RoleShort roleShort,
            DateTime currentDate, DateTime syncTime)
        {
            role.SyncAt = syncTime.ToUniversalTime();

            if (role.Name == roleShort.Name &&
                role.Description == roleShort.Description &&
                role.Status == roleShort.Status &&
                role.IsNeedAprove == roleShort.IsNeedAprove &&
                role.Comment == roleShort.Comment)
                return role;

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

        private Role AddNewRole(Service serviceKey, RoleShort roleShort, DateTime currentdate, DateTime syncTime)
        {
            var role = _mapper.Map<Role>(roleShort);

            role.Service = serviceKey;

            role.UpdateAt = currentdate.ToUniversalTime();
            role.CreateAT = currentdate.ToUniversalTime();
            role.SyncAt = syncTime.ToUniversalTime();

            try
            {
                _applicationDbContext.Roles.Attach(role);
            }
            catch (Exception ex)
            {

            }

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
