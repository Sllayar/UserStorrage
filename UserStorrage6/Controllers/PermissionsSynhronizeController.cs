using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

using UserStorrage6.Model.Context.Repository;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.Rest;
using UserStorrage6.Services.Interfaces;

namespace UserStorrage6.Controllers
{
    [Route("[action]")]
    [ApiController]
    public class PermissionsSynhronizeController : ControllerBase
    {
        private readonly IPermissionBrockerService _permissionService;
        private readonly ISynhronizeService<Permission> _synhronizeService;
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public PermissionsSynhronizeController(
            IPermissionBrockerService permissionService,
            ISynhronizeService<Permission> synhronizeService, 
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _synhronizeService = synhronizeService;
            _permissionService = permissionService;
            _loggerFactory = loggerFactory;
            _configuration = configuration;
        }

        [HttpPost(Name = "Service/Synhronize/Permissions")]
        [ActionName("Service/Synhronize/Permissions")]
        public async Task<Model.Result?> Synhronize(ServiceSyncPermissionRequest request,
        [FromQuery] DateTime? startSyncTime)
        {
            using (var postgreBrocker = new PostgreBrocker(_configuration, _loggerFactory))
            {
                return await _synhronizeService.Synhronize(
                    postgreBrocker,
                    ControllerContext?.RouteData?.Values["action"]?.ToString(),
                    new { ServiceKey = request, SyncTime = startSyncTime },
                    _permissionService.Synhronize(
                        postgreBrocker,
                        request,
                        startSyncTime == null ? DateTime.UtcNow : (DateTime)startSyncTime));

            }
        }


        [HttpPost(Name = "Service/Synhronize/Permissions/Part")]
        [ActionName("Service/Synhronize/Permissions/Part")]
        public async Task<Model.Result> SynhronizePart(ServiceSyncPermissionRequest request,
            [Required][FromQuery] DateTime startSyncTime)
        {
            using (var postgreBrocker = new PostgreBrocker(_configuration, _loggerFactory))
            {
                var task = _permissionService.SynhronizePart(
                    postgreBrocker,
                    request,
                    startSyncTime);

                return await _synhronizeService.Synhronize(
                    postgreBrocker,
                    ControllerContext?.RouteData?.Values["action"]?.ToString(),
                    new { ServiceKey = request, SyncTime = startSyncTime },
                    task);
            }
        }

        [HttpPost(Name = "Service/Synhronize/Permissions/Part/Finish")]
        [ActionName("Service/Synhronize/Permissions/Part/Finish")]
        public async Task<Model.Result> SynhronizePartFinifsh(
            [Required][FromQuery] string sysId,
            [Required][FromQuery] DateTime startSyncTime)
        {
            using (var postgreBrocker = new PostgreBrocker(_configuration, _loggerFactory))
            {
                var task = _permissionService.SynhronizePartFinish(
                    postgreBrocker,
                    sysId,
                    startSyncTime);

                return await _synhronizeService.Synhronize(
                    postgreBrocker,
                    ControllerContext?.RouteData?.Values["action"]?.ToString(),
                    new { SysId = sysId, SyncTime = startSyncTime },
                    task);
            }
        }
    }
}
