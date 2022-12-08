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

        public async Task<Service> Sinhronize(ServiceRequest service)
        {
            var currentService =
                    _applicationDbContext.Services.Include(s => s.Users).FirstOrDefault(dbService =>
                        service.Key == dbService.Key);

            if (currentService == null) currentService = AddService(service);

            var currentdate = DateTime.UtcNow;

            foreach (var user in service.Users)
            {
                var currentUser = currentService?.Users?.FirstOrDefault(u =>
                    u.SysLogin == user.SysLogin);

                if (currentUser == null) AddNewUser(currentService, user, currentdate);
                else UpdateUser(currentUser, user, currentdate);
            }

            DeleteNotUpdatedUsers(currentService, currentdate);

            await _applicationDbContext.SaveChangesAsync();

            return currentService;
        }

        private void DeleteNotUpdatedUsers(Service currentService, DateTime currentdate)
        {
            if (currentService == null || currentService.Users == null) return;

            foreach (var eu in exceptUsers)
            {
                if (user.UpdateAt != currentdate)
                    user.IsActive = Status.Delete;
            }
        }

        private void UpdateUser(User currentUser, UserShort user, DateTime currentdate)
        {
            currentUser.Comment = user.Comment;
            currentUser.DomainLogin = user.DomainLogin;
            currentUser.IsActive = user.IsActive;
            currentUser.SysLogin = user.SysLogin;
            currentUser.Type = user.Type;
            currentUser.UpdateAt = currentdate;
        }

        private User? AddNewUser(Service? currentService,
            UserShort user, DateTime currentdate)
        {
            if (currentService == null) return null;

            var newUser = _mapper.Map<User>(user);
            newUser.UpdateAt = currentdate;
            newUser.CreateAT = currentdate;

            currentService.Users?.Add(newUser);

            return newUser;
        }

        private Service AddService(ServiceRequest service)
        {
            var newService = _mapper.Map<Service>(service);

            _applicationDbContext.Services.Add(newService);

            return newService;
        }
    }
}
