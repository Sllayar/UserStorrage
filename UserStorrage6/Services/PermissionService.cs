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
    public class PermissionService : IPermissionService
    {
        private readonly IServicesService _service;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public PermissionService(
            ApplicationDbContext applicationDbContext,
            IMapper mapper,
            IServicesService service)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _service = service;
        }

        public async Task<List<Permission>?> Synhronize(
            ServiceSyncPermissionRequest service, DateTime SyncDate)
        {
            var currentDate = DateTime.UtcNow;

            var updatedService = await _service.CreateOrUpdate(_mapper.Map<ServiceSyncRequest>(service));

            var existPermissions = _applicationDbContext.Permissions
                .Include(s => s.Users)
                .Include(s => s.Service)
                .Where(p => p.Service.Id == updatedService.Id)
                .ToList();

            foreach (var permission in service.Permissions)
            {
                var curentPermissions = existPermissions?.FirstOrDefault(p => p.SysId == permission.SysId);

                if (curentPermissions == null) CreatePermission(updatedService, permission, currentDate, SyncDate);
                else TryUpdatePermission(curentPermissions, permission, currentDate, SyncDate);
            }

            var exceptPermissions = existPermissions?
                .Select(s => s.SysId)
                .Except(service.Permissions.Select(s => s.SysId))
                .ToList();

            DeleteNotUpdatedPermissions(existPermissions, exceptPermissions, currentDate, SyncDate);

            await _applicationDbContext.SaveChangesAsync();

            return _applicationDbContext.Permissions
                .Include(s => s.Users)
                .Include(s => s.Service)
                .Where(p => p.Service.Id == updatedService.Id)
                .ToList();
        }

        public async Task<List<Permission>?> Synhronize(
            RoleSyncRequest roleSyncRequest, DateTime currentDate, DateTime syncTime)
        {
            if (roleSyncRequest == null || roleSyncRequest.Roles.Count == 0) return new List<Permission>();

            var updatedService = await _service.CreateOrUpdate(_mapper.Map<ServiceSyncRequest>(roleSyncRequest));

            var existPermissions = _applicationDbContext.Permissions
                .Include(s => s.Users)
                .Include(s => s.Service)
                .Where(p => p.Service.Key == roleSyncRequest.Key)
                .ToList();

            var result = new List<Permission>();

            foreach (var role in roleSyncRequest.Roles)
                foreach (var permission in role.Permitions)
            {
                var curentPermissions = existPermissions?.FirstOrDefault(p => p.SysId == permission.SysId);

                if (curentPermissions == null) result.Add(CreatePermission(updatedService, permission, currentDate, syncTime));
                else result.Add(TryUpdatePermission(curentPermissions, permission, currentDate, syncTime));
            }

            await _applicationDbContext.SaveChangesAsync();

            return result;
        }

        public async Task<List<Permission>?> Synhronize(
            List<PermissionShort>? permitions, Service service, DateTime currentDate, DateTime syncTime)
        {
            if (permitions == null || permitions.Count == 0) return new List<Permission>();

            var existPermissions = _applicationDbContext.Permissions
                .Include(s => s.Users)
                .Include(s => s.Service)
                .Where(p => p.Service.Id == service.Id)
                .ToList();

            var result = new List<Permission>();

            foreach (var permission in permitions)
            {
                var curentPermissions = existPermissions?.FirstOrDefault(p => p.SysId == permission.SysId);

                if (curentPermissions == null) result.Add(CreatePermission(service, permission, currentDate, syncTime));
                else result.Add(TryUpdatePermission(curentPermissions, permission, currentDate, syncTime));
            }

            //await _applicationDbContext.SaveChangesAsync();

            return result;
        }

        public Permission TryUpdatePermission(Permission permission,
            PermissionShort newPermission, DateTime currentdate, DateTime syncTime)
        {
            permission.SyncAt = syncTime.ToUniversalTime();

            if (permission.Name == newPermission.Name &&
                permission.Description == newPermission.Description &&
                permission.Status == newPermission.Status &&
                permission.IsNeedAprove == newPermission.IsNeedAprove &&
                permission.Comment == newPermission.Comment)
                return permission;

            permission.Name = newPermission.Name;
            permission.Description = newPermission.Description;
            permission.Status = newPermission.Status;
            permission.IsNeedAprove = newPermission.IsNeedAprove;
            permission.Comment = newPermission.Comment;

            permission.UpdateAt = currentdate.ToUniversalTime();

            permission.Users?.ForEach(u => 
                u.UpdateAt = currentdate.ToUniversalTime());

            return permission;
        }

        public Permission CreatePermission(Service serviceKey,
            PermissionShort newPermission, DateTime currentdate, DateTime syncTime)
        {
            var permission = _mapper.Map<Permission>(newPermission);

            permission.Service = serviceKey;

            permission.UpdateAt = currentdate.ToUniversalTime();
            permission.CreateAT = currentdate.ToUniversalTime();
            permission.SyncAt   = syncTime.ToUniversalTime();

            _applicationDbContext.Permissions.Attach(permission);

            return permission;
        }

        public void DeleteNotUpdatedPermissions(List<Permission>? permissions,
            List<string>? exceptPermissions, DateTime currentdate, DateTime syncTime)
        {
            if (permissions == null) return;

            foreach (var p in exceptPermissions)
            {
                var curPermissions = permissions
                    .FirstOrDefault(ps => ps.SysId == p);

                if (curPermissions != null || curPermissions.Status != Status.Delete)
                {
                    curPermissions.Status = Status.Delete;
                    curPermissions.SyncAt = syncTime.ToUniversalTime();
                    curPermissions.UpdateAt = currentdate.ToUniversalTime();
                }
            }
        }
    }
}
