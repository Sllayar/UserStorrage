using Microsoft.AspNetCore.Mvc;

using UserStorrage6.Model.Context.Repository;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.Rest;
using UserStorrage6.Services.Interfaces;
using UserStorrage6.Model.Requests.GraphQl;
using System.ComponentModel.DataAnnotations;

namespace UserStorrage6.Controllers
{
#if DEBUG
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
        public List<User> GetUsers([FromHeader] string ServiceKey)
        {
            if (_testBrocker.Services.Count == 0) return new List<User>();
            if (string.IsNullOrEmpty(ServiceKey)) return _testBrocker.Services.FirstOrDefault().Users;
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
        public async Task<List<User>?> ServiceSinhronize([Required] ServiceSyncUserRequest service)
        {
            return await _userBrockerService.Sinhronize(_testBrocker, service);
        }

        [HttpPost(Name = "Test/Synhronize/Service/User/Part")]
        [ActionName("Test/Synhronize/Service/User/Part")]
        public async Task<List<User>?> PartServiceSinhronize(
            [Required] ServiceSyncUserRequest service,
            [Required][FromQuery] DateTime startSyncTime)
        {
            return await _userBrockerService.PartSinhronize(
                    _testBrocker, service,
                    startSyncTime == null ? DateTime.UtcNow : (DateTime)startSyncTime);
        }

        [HttpPost(Name = "Test/Synhronize/Service/User/Part/Finish")]
        [ActionName("Test/Synhronize/Service/User/Part/Finish")]
        public async Task<List<User>?> DeleteNotUpdatedUsers(
            [Required][FromQuery] string serviceKey,
            [Required][FromQuery] DateTime startSyncTime)
        {
            return await _userBrockerService.DeleteNotUpdatedUsers(_testBrocker, serviceKey, startSyncTime);
        }

        [HttpPost(Name = "Test/Service/Synhronize/Permissions")]
        [ActionName("Test/Service/Synhronize/Permissions")]
        public async Task<List<Permission>?> Synhronize(
            [Required] ServiceSyncPermissionRequest request,
            [FromQuery] DateTime? startSyncTime)
        {
            return await _permissionService.Synhronize(
                _testBrocker,
                request,
                startSyncTime == null ? DateTime.UtcNow : (DateTime)startSyncTime);
        }

        [HttpPost(Name = "Test/Service/Synhronize/Permissions/Part")]
        [ActionName("Test/Service/Synhronize/Permissions/Part")]
        public async Task<List<Permission>?> SynhronizePermissionsPart(
            [Required] ServiceSyncPermissionRequest request,
            [FromQuery][Required] DateTime? startSyncTime)
        {
            return await _permissionService.SynhronizePart(
                _testBrocker,
                request,
                startSyncTime == null ? DateTime.UtcNow : (DateTime)startSyncTime);
        }

        [HttpPost(Name = "Test/Service/Synhronize/Permissions/Part/Finish")]
        [ActionName("Test/Service/Synhronize/Permissions/Part/Finish")]
        public async Task<List<Permission>?> SynhronizePermissionsPartFinish(
            [FromQuery][Required] string serviceKey,
            [FromQuery][Required] DateTime? startSyncTime)
        {
            return await _permissionService.SynhronizePartFinish(
                _testBrocker,
                serviceKey,
                startSyncTime == null ? DateTime.UtcNow : (DateTime)startSyncTime);
        }


        [HttpPost(Name = "Test/Service/Synhronize/Roles")]
        [ActionName("Test/Service/Synhronize/Roles")]
        public async Task<List<Role>?> Synhronize(
            [Required] RoleSyncRequest request,
            [FromQuery] DateTime? startSyncTime)
        {
            return await _roleService.Synhronize(
                _testBrocker,
                request,
                startSyncTime == null ? DateTime.UtcNow : (DateTime)startSyncTime);
        }

        [HttpPost(Name = "Test/Service/Synhronize/Roles/Part")]
        [ActionName("Test/Service/Synhronize/Roles/Part")]
        public async Task<List<Role>?> SynhronizeRolePart(
            [Required] RoleSyncRequest request,
            [FromQuery][Required] DateTime? startSyncTime)
        {
            return await _roleService.SynhronizePart(
                _testBrocker,
                request,
                startSyncTime == null ? DateTime.UtcNow : (DateTime)startSyncTime);
        }

        [HttpPost(Name = "Test/Service/Synhronize/Roles/Part/Finish")]
        [ActionName("Test/Service/Synhronize/Roles/Part/Finish")]
        public async Task<List<Role>?> SynhronizeRolePartFinish(
            [FromQuery][Required] DateTime? startSyncTime,
            [FromQuery][Required] string serviceKey)
        {
            return await _roleService.SynhronizePartFinish(
                _testBrocker,
                serviceKey,
                startSyncTime == null ? DateTime.UtcNow : (DateTime)startSyncTime);
        }
    }
#endif
}
