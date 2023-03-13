using AutoMapper;

using UserStorrage6.Model.Context.Repository;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.Rest;
using UserStorrage6.Model.Requests.Short;
using UserStorrage6.Services.Interfaces;

namespace UserStorrage6.Services.Brockers
{
    public class UserBrockerService : IUserBrockerService
    {
        private readonly IMapper _mapper;
        private readonly IServicesBrockersService _service;

        public UserBrockerService(
            IMapper mapper, 
            IServicesBrockersService service)
        {
            _mapper = mapper;
            _service = service;
        }

        public async Task<List<User>?> PartSinhronize(
            IDataBrocker dataBrocker, ServiceSyncUserRequest service, DateTime syncDate)
        {
            var updatedService = await _service.CreateOrUpdate(
                dataBrocker, _mapper.Map<ServiceSyncRequest>(service));

            var users = dataBrocker.GetUsers(updatedService.Key);

            var permissions = dataBrocker.GetPermissions(updatedService.Key).ToList();
            var roles = dataBrocker.GetRole(updatedService.Key).ToList();
            CheckForExist(permissions, roles, service);

            var currentDate = DateTime.UtcNow;

            foreach (var user in service.Users)
            {
                var currentUser = users?
                    .FirstOrDefault(u => u.SysId == user.SysId);

                if (currentUser == null)
                    AddNewUser(dataBrocker, updatedService, permissions, roles, user, currentDate, syncDate);
                else 
                    UpdateUser(permissions, roles, currentUser, user, currentDate, syncDate);
            }

            await dataBrocker.SaveChangesAsync();

            foreach (var user in service.Users)
                AddRoleAndPermissions(dataBrocker, updatedService, user);

            await dataBrocker.SaveChangesAsync();

            return dataBrocker.GetServiceWithUser(updatedService.Id).Users?
                .Where(u => service.Users.Exists(user => user.SysId == u.SysId))
                .ToList();
        }

        public async Task<List<User>?> DeleteNotUpdatedUsers(
            IDataBrocker dataBrocker, string serviceKey, DateTime syncDate)
        {
            var currentService = dataBrocker.GetServiceWithUser(serviceKey);

            var exceptUsers = currentService?.Users?
               .Where(u => u.PartSyncAt != syncDate.ToUniversalTime())
               .Select(s => s.SysId)
               .ToList();

            var currentDate = DateTime.UtcNow;

            DeleteNotUpdatedUsers(currentService.Users.AsQueryable(), exceptUsers, currentDate, syncDate);

            await dataBrocker.SaveChangesAsync();

            return currentService?.Users?.ToList(); ;
        }

        public async Task<List<User>?> Sinhronize(
            IDataBrocker dataBrocker, ServiceSyncUserRequest service)
        {
            var updatedService = await _service.CreateOrUpdate(
                dataBrocker, _mapper.Map<ServiceSyncRequest>(service));

            var users = dataBrocker.GetUsers(updatedService.Key);

            var permissions = dataBrocker.GetPermissions(updatedService.Key).ToList();
            var roles = dataBrocker.GetRole(updatedService.Key).ToList();
            CheckForExist(permissions, roles, service);

            var currentDate = DateTime.UtcNow;

            foreach (var user in service.Users)
            {
                var currentUser = users.FirstOrDefault(u => u.SysId == user.SysId);

                if (currentUser == null) 
                    AddNewUser(dataBrocker, updatedService, permissions, roles, user, currentDate, currentDate);
                else 
                    UpdateUser(permissions, roles, currentUser, user, currentDate, currentDate);
            }

            var exceptUsers = users
                .Select(s => s.SysId)
                .ToList()
                .Except(service.Users.Select(s => s.SysId))
                .ToList();

            DeleteNotUpdatedUsers(users, exceptUsers, currentDate, currentDate);

            await dataBrocker.SaveChangesAsync();

            return dataBrocker.GetServiceWithUser(updatedService.Id).Users;
        }

        private void CheckForExist(List<Permission> permissions, List<Role> roles, ServiceSyncUserRequest service)
        {
            foreach (var user in service.Users)
            {
                if (user.Roles.Except(roles.Select(r => r.SysId)).Count() > 0 ||
                    user.Permissions.Except(permissions.Select(p => p.SysId)).Count() > 0)
                    throw new Exception($"Обнаружена попытка добавить несуществующую роль или несуществующее право к пользователю {user.SysId}");
            }
        }

        private void DeleteNotUpdatedUsers(IQueryable<User> users,
            List<string> exceptUsers, DateTime currentdate, DateTime syncTime)
        {
            if (users == null) return;

            foreach (var eu in exceptUsers)
            {
                var curUser = users
                    .FirstOrDefault(u => u.SysId == eu);

                if (curUser == null && curUser.Status == Status.Delete) continue;

                curUser.Status = Status.Delete;
                curUser.SyncAt = currentdate.ToUniversalTime();
                curUser.UpdateAt = syncTime.ToUniversalTime();
                curUser.PartSyncAt = syncTime.ToUniversalTime();
            }
        }

        private void UpdateUser(List<Permission> permissions, List<Role> roles, User currentUser, UserShort user,
            DateTime currentdate, DateTime syncTime)
        {
            currentUser.SyncAt = currentdate.ToUniversalTime();
            currentUser.PartSyncAt = syncTime.ToUniversalTime();

            var delPermissions = currentUser.Permissions.Select(s => s.SysId).Except(user.Permissions).ToList();
            var delRole = currentUser.Roles.Select(s => s.SysId).Except(user.Roles).ToList();
            var addPermissions = user.Permissions.Except(currentUser.Permissions.Select(p => p.SysId)).ToList();
            var addRoles = user.Roles.Except(currentUser.Roles.Select(p => p.SysId)).ToList();

            if (currentUser.Comment == user.Comment &&
                currentUser.OwnerLogin == user.OwnerLogin &&
                currentUser.Status == user.Status &&
                currentUser.SysLogin == user.SysLogin &&
                currentUser.Type == user.Type &&
                delPermissions.Count == 0 &&
                delRole.Count == 0 &&
                addPermissions.Count == 0 &&
                addRoles.Count == 0
                )
                return;

            addPermissions.ForEach(p => 
                currentUser.Permissions.Add(permissions.Find(p1 => p1.SysId == p)));

            addRoles.ForEach(r =>
                currentUser.Roles.Add(roles.Find(r1 => r1.SysId == r)));

            delPermissions.ForEach(p =>
                currentUser.Permissions.Remove(permissions.Find(p1 => p1.SysId == p)));

            delRole.ForEach(r =>
                currentUser.Roles.Remove(roles.Find(r1 => r1.SysId == r)));

            currentUser.Comment = user.Comment;
            currentUser.OwnerLogin = user.OwnerLogin;
            currentUser.Status = user.Status;
            currentUser.SysLogin = user.SysLogin;
            currentUser.Type = user.Type;
            currentUser.UpdateAt = syncTime.ToUniversalTime();
        }

        private User? AddNewUser(IDataBrocker dataBrocker, Service service, List<Permission> permissions,
            List<Role> roles, UserShort user, DateTime currentdate, DateTime syncTime)
        {
            var newUser = _mapper.Map<User>(user);
            newUser.CreateAT = currentdate.ToUniversalTime();
            newUser.SyncAt = currentdate.ToUniversalTime();
            newUser.UpdateAt = syncTime.ToUniversalTime();
            newUser.PartSyncAt = syncTime.ToUniversalTime();

            newUser.Service = service;

            user.Permissions.ToList().ForEach(p => newUser.Permissions.Add(permissions.FirstOrDefault(p1 => p1.SysId == p)));
            user.Roles.ToList().ForEach(r => newUser.Roles.Add(roles.FirstOrDefault(r1 => r1.SysId == r)));

            dataBrocker.AttachUser(newUser);

            return newUser;
        }

        private void AddRoleAndPermissions(
            IDataBrocker dataBrocker, Service service, UserShort userReq)
        {
            var user = dataBrocker.GetUser(userReq.SysId, service.Key);

            AddPermissions(user, dataBrocker, service, userReq);
            AddRoles(user, dataBrocker, service, userReq);
        }

        private void AddPermissions(User? user,
            IDataBrocker dataBrocker, Service service, UserShort userReq)
        {
            if (userReq?.Permissions == null) return;

            var permissions = dataBrocker
                .GetPermissions(service.Key)
                .ToList();

            foreach (var permission in userReq.Permissions)
            {
                var newPermission = permissions.FirstOrDefault(p => p.SysId == permission);

                if (newPermission == null) throw new Exception($"Несуществующий параметр {permission}");
                if (user.Permissions.FirstOrDefault(p => p.SysId == newPermission.SysId) != null) continue;
                else user.Permissions.Add(newPermission);
            }
        }

        private void AddRoles(User? user,
            IDataBrocker dataBrocker, Service service, UserShort userReq)
        {
            if (userReq?.Roles == null) return;

            var roles = dataBrocker
                .GetRole(service.Key)
                .ToList();

            foreach (var role in userReq.Roles)
            {
                var newRole = roles.FirstOrDefault(p => p.SysId == role);

                if (newRole == null) throw new Exception($"Несуществующий параметр {role}");
                if (user.Roles.FirstOrDefault(p => p.SysId == newRole.SysId) != null) continue;
                else user.Roles.Add(newRole);
            }
        }
    }
}
