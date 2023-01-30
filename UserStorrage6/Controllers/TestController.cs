using Microsoft.AspNetCore.Mvc;

using UserStorrage6.Model.Context.Repository;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.Rest;
using UserStorrage6.Services.Interfaces;
using UserStorrage6.Model.Requests.GraphQl;
using System.ComponentModel.DataAnnotations;

namespace UserStorrage6.Controllers
{
    [Route("[action]")]
    [ApiController]
    public class TestController : Controller
    {
        private readonly TestBrocker _testBrocker;

        private readonly IUserBrockerService _userBrockerService;
        private readonly IRoleBrockerService _roleService;
        private readonly IPermissionBrockerService _permissionService;

        public TestController(
            TestBrocker testBrocker, 
            IUserBrockerService userBrockerService,
            IPermissionBrockerService permissionService,
            IRoleBrockerService roleBrockerService)
        {
            _testBrocker = testBrocker;
            _userBrockerService = userBrockerService;
            _permissionService = permissionService;
            _roleService = roleBrockerService;
        }

        [HttpGet(Name = "Test/Clear")]
        [ActionName("Test/Clear")]
        public void Clear() => _testBrocker.Dispose();
        
        [HttpGet(Name = "Test/History")]
        [ActionName("Test/History")]
        public List<History> GetHistories() 
        {
            return _testBrocker.Historys;
        }

        [HttpGet(Name = "Test/Services")]
        [ActionName("Test/Services")]
        public List<Service> GetServices()
        {
            return _testBrocker.Services;
        }

        [HttpGet(Name = "Test/Users")]
        [ActionName("Test/Users")]
        public List<User> GetUsers([FromHeader]string ServiceKey)
        {
            if (_testBrocker.Services.Count == 0) return new List<User>();
            if(string.IsNullOrEmpty(ServiceKey)) return _testBrocker.Services.FirstOrDefault().Users;
            return _testBrocker.Services.FirstOrDefault(s => s.Key == ServiceKey).Users;
        }

        [HttpGet(Name = "Test/Roles")]
        [ActionName("Test/Roles")]
        public List<Role> GetRoles()
        {
            return _testBrocker.Roles;
        }

        [HttpGet(Name = "Test/Permissions")]
        [ActionName("Test/Permissions")]
        public List<Permission> GetPermissions()
        {
            return _testBrocker.Permissions;
        }

        [HttpPost(Name = "Test/Synhronize/Service/User")]
        [ActionName("Test/Synhronize/Service/User")]
        public async Task<List<User>?> ServiceSinhronize(ServiceSyncUserRequest service)
        {
            return await _userBrockerService.Sinhronize(_testBrocker, service);
        }

        [HttpDelete(Name = "Test/Synhronize/Service/User")]
        [ActionName("Test/Synhronize/Service/User")]
        public async Task<List<User>?> DeleteNotUpdatedUsers(
            [Required][FromQuery] string serviceKey,
            [Required][FromQuery] DateTime statrtSyncTime)
        {
            return await _userBrockerService.DeleteNotUpdatedUsers(_testBrocker, serviceKey, statrtSyncTime);
        }


        [HttpPost(Name = "Test/Synhronize/Service/User/Part")]
        [ActionName("Test/Synhronize/Service/User/Part")]
        public async Task<List<User>?> PartServiceSinhronize(
            ServiceSyncUserRequest service,
            [Required][FromQuery] DateTime statrtSyncTime)
        {
            return await _userBrockerService.PartSinhronize(
                    _testBrocker, service,
                    statrtSyncTime == null ? DateTime.UtcNow : (DateTime)statrtSyncTime);
        }

        [HttpPost(Name = "Test/Service/Synhronize/Permissions")]
        [ActionName("Test/Service/Synhronize/Permissions")]
        public async Task<List<Permission>?> Synhronize(ServiceSyncPermissionRequest request,
        [FromQuery] DateTime? statrtSyncTime)
        {
            return await _permissionService.Synhronize(
                _testBrocker,
                request,
                statrtSyncTime == null ? DateTime.UtcNow : (DateTime)statrtSyncTime);
        }


        [HttpPost(Name = "Test/Service/Synhronize/Roles")]
        [ActionName("Test/Service/Synhronize/Roles")]
        public async Task<List<Role>?> Synhronize(RoleSyncRequest request,
                    [FromQuery] DateTime? statrtSyncTime)
        {
            return await _roleService.Synhronize(
                _testBrocker,
                request,
                statrtSyncTime == null ? DateTime.UtcNow : (DateTime)statrtSyncTime);
        }
    }
}
