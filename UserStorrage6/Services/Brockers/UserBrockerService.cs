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

            var currentService = dataBrocker.GetServiceWithUser(updatedService.Id);

            var currentDate = DateTime.UtcNow;

            foreach (var user in service.Users)
            {
                var currentUser = currentService?.Users?
                    .FirstOrDefault(u => u.SysId == user.SysId);

                if (currentUser == null) AddNewUser(dataBrocker, currentService, user, currentDate, syncDate);
                else UpdateUser(currentUser, user, currentDate, syncDate);
            }

            await dataBrocker.SaveChangesAsync();

            foreach (var user in service.Users)
                AddRoleAndPermissions(dataBrocker, updatedService, user);

            await dataBrocker.SaveChangesAsync();

            return dataBrocker.GetServiceWithUser(updatedService.Id).Users?
                .Where(u =>
                    service.Users.Exists(user => user.SysId == u.SysId))
                .ToList();
        }

        public async Task<List<User>?> DeleteNotUpdatedUsers(
            IDataBrocker dataBrocker, string serviceKey, DateTime syncDate)
        {
            var currentService = dataBrocker.GetServiceWithUser(serviceKey);

            var exceptUsers = currentService?.Users?
               .Where(u => u.SyncAt != syncDate.ToUniversalTime())
               .Select(s => s.SysId)
               .ToList();

            var currentDate = DateTime.UtcNow;

            DeleteNotUpdatedUsers(currentService, exceptUsers, currentDate, syncDate);

            await dataBrocker.SaveChangesAsync();

            return currentService?.Users?.Where(u =>
                u.UpdateAt == syncDate.ToUniversalTime() && u.Status == Status.Delete).ToList(); ;
        }

        public async Task<List<User>?> Sinhronize(
            IDataBrocker dataBrocker, ServiceSyncUserRequest service)
        {
            var updatedService = await _service.CreateOrUpdate(
                dataBrocker, _mapper.Map<ServiceSyncRequest>(service));

            var currentService = dataBrocker.GetServiceWithUser(updatedService.Id);

            var currentDate = DateTime.UtcNow;

            foreach (var user in service.Users)
            {
                var currentUser = currentService?.Users?
                    .FirstOrDefault(u => u.SysId == user.SysId);

                if (currentUser == null) AddNewUser(dataBrocker, currentService, user, currentDate, currentDate);
                else UpdateUser(currentUser, user, currentDate, currentDate);
            }

            var exceptUsers = currentService?.Users?
                .Select(s => s.SysId)
                .Except(service.Users.Select(s => s.SysId))
                .ToList();

            DeleteNotUpdatedUsers(currentService, exceptUsers, currentDate, currentDate);

            await dataBrocker.SaveChangesAsync();

            foreach (var user in service.Users)
                AddRoleAndPermissions(dataBrocker, updatedService, user);

            await dataBrocker.SaveChangesAsync();

            return dataBrocker.GetServiceWithUser(updatedService.Id).Users;
        }

        private void DeleteNotUpdatedUsers(Service currentService,
            List<string> exceptUsers, DateTime currentdate, DateTime syncTime)
        {
            if (currentService == null || currentService.Users == null) return;

            foreach (var eu in exceptUsers)
            {
                var curUser = currentService.Users
                    .FirstOrDefault(u => u.SysId == eu);

                if (curUser == null && curUser.Status == Status.Delete) continue;

                curUser.Status = Status.Delete;
                curUser.SyncAt = syncTime.ToUniversalTime();
                curUser.UpdateAt = currentdate.ToUniversalTime();
            }
        }

        private void UpdateUser(User currentUser, UserShort user,
            DateTime currentdate, DateTime syncTime)
        {
            currentUser.SyncAt = syncTime.ToUniversalTime();

            if (currentUser.Comment == user.Comment &&
                currentUser.OwnerLogin == user.OwnerLogin &&
                currentUser.Status == user.Status &&
                currentUser.SysLogin == user.SysLogin &&
                currentUser.Type == user.Type)
                return;

            currentUser.Comment = user.Comment;
            currentUser.OwnerLogin = user.OwnerLogin;
            currentUser.Status = user.Status;
            currentUser.SysLogin = user.SysLogin;
            currentUser.Type = user.Type;
            currentUser.UpdateAt = currentdate.ToUniversalTime();
        }

        private User? AddNewUser(IDataBrocker dataBrocker, Service? currentService,
            UserShort user, DateTime currentdate, DateTime syncTime)
        {
            if (currentService == null) return null;

            var newUser = _mapper.Map<User>(user);
            newUser.UpdateAt = currentdate.ToUniversalTime();
            newUser.CreateAT = currentdate.ToUniversalTime();
            newUser.SyncAt = syncTime.ToUniversalTime();

            currentService.Users?.Add(newUser);

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
                else user.Roles.Add(newRole);
            }
        }
    }
}
