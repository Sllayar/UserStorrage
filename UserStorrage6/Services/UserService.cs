using AutoMapper;

using Microsoft.EntityFrameworkCore;

using UsersStorrage.Models.Context;

using UserStorrage6.Model.DB;
using UserStorrage6.Model.Short;

namespace UserStorrage6.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public UserService(
            ApplicationDbContext applicationDbContext,
            IMapper mapper) 
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }
        public async Task<Service> PartSinhronize(ServiceRequest service, DateTime syncDate)
        {
            var currentService =
                _applicationDbContext.Services
                .Include(s => s.Users)
                .FirstOrDefault(dbService => service.Key == dbService.Key);

            if (currentService == null) currentService = AddService(service);
            else UpdateService(service, currentService);

            var currentDate = DateTime.UtcNow;

            foreach (var user in service.Users)
            {
                var currentUser = currentService?.Users?
                    .FirstOrDefault(u => u.SysId == user.SysId);

                if (currentUser == null) AddNewUser(currentService, user, currentDate, syncDate);
                else UpdateUser(currentUser, user, currentDate, syncDate);
            }

            await _applicationDbContext.SaveChangesAsync();

            return currentService;
        }

        public async Task<Service> DeleteNotUpdatedUsers(string serviceKey, DateTime syncDate)
        {
            var currentService =
                _applicationDbContext.Services
                .Include(s => s.Users)
                .FirstOrDefault(dbService => serviceKey == dbService.Key);

            var exceptUsers = currentService?.Users?
               .Where(u => u.SyncAt != syncDate.ToUniversalTime())
               .Select(s => s.SysId)
               .ToList();

            var currentDate = DateTime.UtcNow;

            DeleteNotUpdatedUsers(currentService, exceptUsers, currentDate, syncDate);

            await _applicationDbContext.SaveChangesAsync();

            return currentService;
        }

            public async Task<Service> Sinhronize(ServiceRequest service)
        {
            var currentService =
                _applicationDbContext.Services
                .Include(s => s.Users)
                .FirstOrDefault(dbService => service.Key == dbService.Key);

            if (currentService == null) currentService = AddService(service);
            else UpdateService(service, currentService);

            var currentDate = DateTime.UtcNow;

            foreach (var user in service.Users)
            {
                var currentUser = currentService?.Users?
                    .FirstOrDefault(u => u.SysId == user.SysId);

                if (currentUser == null) AddNewUser(currentService, user, currentDate, currentDate);
                else UpdateUser(currentUser, user, currentDate, currentDate);
            }

            var exceptUsers = currentService?.Users?
                .Select(s => s.SysId)
                .Except(service.Users.Select(s => s.SysId))
                .ToList();

            DeleteNotUpdatedUsers(currentService, exceptUsers, currentDate, currentDate);

            await _applicationDbContext.SaveChangesAsync();

            return currentService;
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
                currentUser.Type == user.Type )
                return;

            currentUser.Comment = user.Comment;
            currentUser.OwnerLogin = user.OwnerLogin;
            currentUser.Status = user.Status;
            currentUser.SysLogin = user.SysLogin;
            currentUser.Type = user.Type;
            currentUser.UpdateAt = currentdate.ToUniversalTime();
        }

        private User? AddNewUser(Service? currentService,
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

        private Service AddService(ServiceRequest service)
        {
            var newService = _mapper.Map<Service>(service);

            _applicationDbContext.Services.Add(newService);

            return newService;
        }

        private void UpdateService(ServiceRequest service, Service updated)
        {
            updated.Name = service.Name;
            updated.Status = service.Status;
            updated.Author = service.Author;
            updated.Contacts = service.Contacts;
        }
    }
}
