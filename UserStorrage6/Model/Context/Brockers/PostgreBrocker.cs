using HotChocolate;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using System.Linq;
using System.Linq.Expressions;
using System.Security;

using UsersStorrage.Models.Context;

using UserStorrage6.Model.DB;

namespace UserStorrage6.Model.Context.Repository
{
    public interface IDataBrocker : IDisposable
    {
        void AddHsitorryAsync(History history);

        void AddServiceAsync(Service history);

        void AttachPermission(Permission permission);

        void AttachRole(Role role);

        void AttachUser(User role);

        Task<History> GetHistoryByIdAsync(int id);

        Service? GetServiceByIdAsync(string key);

        Service? GetServiceWithUser(int id);

        Service? GetServiceWithUser(string key);
        
        User? GetUser(string key, string ServiceKey);

        IQueryable<User>? GetUsers(string ServiceKey);

        IQueryable<Permission>? GetPermissions(string key);

        IQueryable<Permission>? GetPermissionsFull(int serviceId);

        IQueryable<Role>? GetRoleFull(int serviceId);

        IQueryable<Role>? GetRole(string key);

        IQueryable<Permission>? GetPermissions(List<string> keys);

        Task SaveChangesAsync();
    }

    public class TestBrocker : IDataBrocker
    {
        public List<Service> Services { get; set; } = new List<Service>();
        public List<History> Historys { get; set; } = new List<History>();
        public List<Role> Roles { get; set; } = new List<Role>();
        public List<Permission> Permissions { get; set; } = new List<Permission>();

        public void AddHsitorryAsync(History history)
        {
            history.Id = Historys.Count == 0 ? 1 : Historys.Max(h => h.Id) + 1;
            Historys.Add(history);
        }

        public void AddServiceAsync(Service service)
        {
            service.Id = Services.Count == 0 ? 1 : Services.Max(h => h.Id) + 1;
            Services.Add(service);
        }

        public void AttachPermission(Permission permission)
        {
            permission.Id = Permissions.Count == 0 ? 1 : Permissions.Max(h => h.Id) + 1;
            Permissions.Add(permission);
        }

        public void AttachRole(Role role)
        {
            if (Roles.Count > 0)
                role.Id = Roles.Max(r => r.Id) + 1;
            Roles.Add(role);
        }

        public void AttachUser(User user)
        {
            var service = Services.First(s => s.Key == user.Service.Key);

            if (service.Users.Count > 0)
                user.Id = service.Users.Max(u => u.Id) + 1;
            service.Users.Add(user);
        }

        public void Dispose()
        {
            Services.Clear();
            Historys.Clear();
            Roles.Clear();
            Permissions.Clear();
        }

        public async Task<History> GetHistoryByIdAsync(int id)
        {
            return Historys.FirstOrDefault(h => h.Id == id);
        }

        public IQueryable<Permission>? GetPermissions(string key)
        {
            return Permissions.AsQueryable().Where(p => p.Service.Key == key);
        }

        public IQueryable<Permission>? GetPermissions(List<string>? keys)
        {
            return Permissions.AsQueryable().Where(p => keys.FirstOrDefault(p.SysId) != null);
        }

        public IQueryable<Permission>? GetPermissionsFull(int serviceId)
        {
            return Permissions.AsQueryable().Where(p => p.Service.Id == serviceId);
        }

        public IQueryable<Role>? GetRole(string key)
        {
            return Roles.AsQueryable().Where(r => r.Service.Key == key);
        }

        public IQueryable<Role>? GetRoleFull(int serviceId)
        {
            return Roles.AsQueryable().Where(r => r.Service.Id == serviceId);
        }

        public Service? GetServiceByIdAsync(string key)
        {
            return Services.FirstOrDefault(h => h.Key == key);
        }

        public Service? GetServiceWithUser(int id)
        {
            return Services.FirstOrDefault(h => h.Id == id);
        }

        public Service? GetServiceWithUser(string key)
        {
            return Services.FirstOrDefault(h => h.Key == key);
        }

        public User? GetUser(string key, string ServiceKey)
        {
            return Services.FirstOrDefault(s => s.Key == ServiceKey).Users.FirstOrDefault(u => u.SysId == key);
        }

        public IQueryable<User>? GetUsers(string ServiceKey)
        {
            return Services.FirstOrDefault(s => s.Key == ServiceKey).Users.AsQueryable();
        }

        public async Task SaveChangesAsync()
        {
            
        }
    }


    public class PostgreBrocker : IDataBrocker
    {
        private ApplicationDbContext _applicationDbContext;

        public PostgreBrocker(IConfiguration config, ILoggerFactory loggerFactory) =>
            _applicationDbContext = new ApplicationDbContext(config, loggerFactory);

        public void Dispose()
        {
            if (_applicationDbContext != null)
            {
                _applicationDbContext.Dispose();
                _applicationDbContext = null;
            }
        }

        public IQueryable<Permission> GetPermissions(string key)
        {
            return _applicationDbContext.Permissions
                .Where(p => p.Service.Key == key);
        }

        public IQueryable<Permission> GetPermissionsFull(int serviceId)
        {
            return _applicationDbContext.Permissions
                .Include(s => s.Users)
                .Include(s => s.Service)
                .Where(p => p.Service.Id == serviceId);
        }

        public IQueryable<Role> GetRole(string key)
        {
            return _applicationDbContext.Roles
                .Include(r => r.SysPermissions)
                .Where(r => r.Service.Key == key);
        }

        public IQueryable<Role> GetRoleFull(int serviceId)
        {
            return _applicationDbContext.Roles
                .Include(r => r.Users)
                .Include(r => r.SysPermissions)
                .Include(r => r.Service)
                .Where(r => r.Service.Id == serviceId);
        }


        public User? GetUser(string key, string ServiceKey)
        {
            return _applicationDbContext.Users
                .Include(u => u.Roles)
                .Include(u => u.Permissions)
                .Include(u => u.Service)
                .FirstOrDefault(u => u.SysId == key && u.Service.Key == ServiceKey);
        }

        public IQueryable<User>? GetUsers(string ServiceKey)
        {
            return _applicationDbContext.Users
                .Include(u => u.Roles)
                .Include(u => u.Permissions)
                .Include(u => u.Service)
                .Where(u => u.Service.Key == ServiceKey);
        }

        public Service? GetServiceWithUser(int serviceId)
        {
            return _applicationDbContext.Services
                .Include(r => r.Users)
                .FirstOrDefault(s => s.Id == serviceId);
        }

        public Service? GetServiceWithUser(string key)
        {
            return _applicationDbContext.Services
                .Include(r => r.Users)
                .FirstOrDefault(s => s.Key == key);
        }

        public void AttachUser(User user)
        {
            _applicationDbContext.Users.Attach(user);
        }

        public void AttachRole(Role role)
        {
            _applicationDbContext.Roles.Attach(role);
        }

        public void AttachPermission(Permission permission)
        {
            _applicationDbContext.Permissions.Attach(permission);
        }

        public void AddHsitorryAsync(History history)
        {
            _applicationDbContext.Historys.Add(history);
        }

        public void AddServiceAsync(Service service)
        {
            _applicationDbContext.Services.Add(service);
        }

        public async Task SaveChangesAsync()
        {
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<History> GetHistoryByIdAsync(int id)
        {
            return await _applicationDbContext.Historys
                .FirstAsync(h => h.Id == id);
        }

        public Service? GetServiceByIdAsync(string key)
        {
            return _applicationDbContext.Services
                .FirstOrDefault(dbService => dbService.Key == key);
        }

        public IQueryable<Permission>? GetPermissions(List<string>? keys)
        {
            return _applicationDbContext.Permissions.AsQueryable().Where(p => 
                keys.FirstOrDefault(p.SysId) != null);
        }
    }
}
