using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.ComponentModel.DataAnnotations;

using UserStorrage6.Model.Context.Repository;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.Rest;
using UserStorrage6.Services.Interfaces;

namespace UserStorrage6.Controllers
{
    [ApiController]
    [Route("[action]")]
    public class UsersSynhronizeController : ControllerBase
    {
        private readonly IUserBrockerService _userBrockerService;
        private readonly ISynhronizeService<User> _synhronizeService;
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public UsersSynhronizeController(
            IUserBrockerService userBrockerService,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            ISynhronizeService<User> synhronizeService)
        {
            _synhronizeService = synhronizeService;
            _userBrockerService = userBrockerService;
            _loggerFactory = loggerFactory;
            _configuration = configuration;
        }

        [HttpPost(Name = "Synhronize/Service/User/Part")]
        [ActionName("Synhronize/Service/User/Part")]
        public async Task<Model.Result> PartServiceSinhronize(
            ServiceSyncUserRequest service,
            [Required][FromQuery] DateTime statrtSyncTime)
        {
            using (var postgreBrocker = new PostgreBrocker(_configuration, _loggerFactory))
            {
                var res = _userBrockerService.PartSinhronize(
                        postgreBrocker, service,
                        statrtSyncTime == null ? DateTime.UtcNow : (DateTime)statrtSyncTime);

                return await _synhronizeService.Synhronize(
                    postgreBrocker,
                    ControllerContext?.RouteData?.Values["action"]?.ToString(),
                    new { Service = service, SyncTime = statrtSyncTime },
                    res);
            }
        }

        [HttpPost(Name = "Synhronize/Service/User/Part/Finish")]
        [ActionName("Synhronize/Service/User/Part/Finish")]
        public async Task<Model.Result> DeleteNotUpdatedUsers(
            [Required][FromQuery] string serviceKey,
            [Required][FromQuery] DateTime statrtSyncTime)
        {
            using (var postgreBrocker = new PostgreBrocker(_configuration, _loggerFactory))
            {
                var res = _userBrockerService.DeleteNotUpdatedUsers(postgreBrocker, serviceKey, statrtSyncTime);

                return await _synhronizeService.Synhronize(
                    postgreBrocker,
                    ControllerContext?.RouteData?.Values["action"]?.ToString(),
                    new { ServiceKey = serviceKey, SyncTime = statrtSyncTime },
                    res);
            }
        }

        [HttpPost( Name = "Synhronize/Service/User")]
        [ActionName("Synhronize/Service/User")]
        public async Task<Model.Result> ServiceSinhronize(ServiceSyncUserRequest service)
        {
            using (var postgreBrocker = new PostgreBrocker(_configuration, _loggerFactory))
            {
                var res = _userBrockerService.Sinhronize(postgreBrocker, service);

                return await _synhronizeService.Synhronize(
                    postgreBrocker,
                    ControllerContext?.RouteData?.Values["action"]?.ToString(),
                    new { Service = service},
                    res);
            }
        }
    }
}
