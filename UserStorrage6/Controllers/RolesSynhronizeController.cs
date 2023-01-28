using Microsoft.AspNetCore.Mvc;

using UserStorrage6.Model.Context.Repository;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.GraphQl;
using UserStorrage6.Services.Interfaces;

namespace UserStorrage6.Controllers
{
    [Route("[action]")]
    [ApiController]
    public class RolesSynhronizeController : ControllerBase
    {
        private readonly IRoleBrockerService _roleService;
        private readonly ISynhronizeService<Role> _synhronizeService;
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public RolesSynhronizeController(
            ISynhronizeService<Role> synhronizeService,
            IRoleBrockerService roleService,
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _synhronizeService = synhronizeService;
            _roleService = roleService;
            _loggerFactory = loggerFactory;
            _configuration = configuration;
        }

        [HttpPost(Name = "Service/Synhronize/Roles")]
        [ActionName("Service/Synhronize/Roles")]
        public async Task<Model.Result> Synhronize(RoleSyncRequest request,
            [FromQuery] DateTime? statrtSyncTime)
        {
            using (var postgreBrocker = new PostgreBrocker(_configuration, _loggerFactory))
            {
                return await _synhronizeService.Synhronize(
                    postgreBrocker,
                    ControllerContext?.RouteData?.Values["action"]?.ToString(),
                    new { ServiceKey = request, SyncTime = statrtSyncTime },
                    await _roleService.Synhronize(
                        postgreBrocker,
                        request,
                        statrtSyncTime == null ? DateTime.UtcNow : (DateTime)statrtSyncTime));
            }
        }
    }
}
