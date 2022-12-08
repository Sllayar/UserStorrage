using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using UsersStorrage.Models.Context;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Short;
using UserStorrage6.Services;

namespace UserStorrage6.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SynhronizeController : ControllerBase
    {
        private readonly ILogger<SynhronizeController> _logger;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _applicationDbContext;

        public SynhronizeController(
            ILogger<SynhronizeController> logger,
            IUserService userService,
            ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _userService = userService;
            _applicationDbContext = applicationDbContext;
        }

        [HttpPost(Name = "users")]
        public async Task<Model.Result> UserSinhronize(ServiceRequest service)
        {
            _logger.Log(LogLevel.Information, "Start UserSinhronize");

            Model.Result result = new Model.Result();

            try
            {
                Validate(service);

                int historryId = await AddHsitorry(service);

                result.Data = JsonConvert.SerializeObject(
                    await _userService.Sinhronize(service));

                await UpdateHistorry(historryId);

                result.Status = "Sucsses";

                _logger.Log(LogLevel.Information, "UserSinhronize Sucsses");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Fail {ex.Message}");

                result.Message = ex.Message;
                result.Status = "Error";
            }

            return result;
        }

        private async Task UpdateHistorry(int historryId)
        {
            var historry = await _applicationDbContext.Historys
                .FirstAsync(h => h.Id == historryId);

            historry.Result = "Sucsses";

            await _applicationDbContext.SaveChangesAsync();
        }

        private async Task<int> AddHsitorry(ServiceRequest service)
        {
            var newHistorry = new History()
            {
                Data = JsonConvert.SerializeObject(service),
                CreateAT = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                Name = "UserSinhronize"
            };

            await _applicationDbContext.Historys.AddAsync(newHistorry);

            await _applicationDbContext.SaveChangesAsync();

            return newHistorry.Id;
        }

        private void Validate(ServiceRequest service)
        {
            if (service == null)
                throw new ArgumentException("Отсутствует ServiceRequest");
            if (service.Users == null || service.Users.Count == 0)
                throw new ArgumentException("Отсутствют полользователи");
            if (string.IsNullOrEmpty(service.Key))
                throw new ArgumentException("Отсутствет поле Key");
            if (string.IsNullOrEmpty(service.Name))
                throw new ArgumentException("Отсутствет поле Name");

            foreach (var user in service.Users)
            {
                if (user?.Status == null)
                    throw new ArgumentException("Отсутствет поле user.Status");
                if (string.IsNullOrEmpty(user?.SysLogin))
                    throw new ArgumentException("Отсутствет поле user.SysLogin");
                if (user?.Type == null)
                    throw new ArgumentException("Отсутствет поле user.Type");
            }
        }
    }
}
