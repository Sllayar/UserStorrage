using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using UsersStorrage.Models.Context;

using UserStorrage6.Model;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.Rest;
using UserStorrage6.Services.Interfaces;

namespace UserStorrage6.Controllers
{
    //[ApiController]
    //[Route("[action]")]
    //public class UsersSynhronizeController : ControllerBase
    //{
    //    private readonly ILogger<UsersSynhronizeController> _logger;
    //    private readonly IUserService _userService;
    //    private readonly ApplicationDbContext _applicationDbContext;

    //    public UsersSynhronizeController(
    //        ILogger<UsersSynhronizeController> logger,
    //        IUserService userService,
    //        ApplicationDbContext applicationDbContext)
    //    {
    //        _logger = logger;
    //        _userService = userService;
    //        _applicationDbContext = applicationDbContext;
    //    }

    //    [HttpPost(Name = "Service/User/Part")]
    //    [ActionName("Service/User/Part")]
    //    public async Task<Model.Result> PartServiceSinhronize(
    //        ServiceSyncUserRequest service,
    //        [Required][FromQuery] DateTime statrtSyncTime)
    //    {
    //        return await Start(
    //            ControllerContext?.RouteData?.Values["action"]?.ToString(),
    //            service,
    //            _userService.PartSinhronize(service, statrtSyncTime));
    //    }

    //    [HttpDelete(Name = "Service/User")]
    //    [ActionName("Service/User")]
    //    public async Task<Model.Result> DeleteNotUpdatedUsers(
    //        [Required][FromQuery] string serviceKey,
    //        [Required][FromQuery] DateTime statrtSyncTime)
    //    {
    //        return await Start(
    //            ControllerContext?.RouteData?.Values["action"]?.ToString(),
    //            new { ServiceKey = serviceKey, SyncTime = statrtSyncTime },
    //            _userService.DeleteNotUpdatedUsers(serviceKey, statrtSyncTime));
    //    }

    //    [HttpPost( Name = "Synhronize/Service/User")]
    //    [ActionName("Synhronize/Service/User")]
    //    public async Task<Model.Result> ServiceSinhronize(ServiceSyncUserRequest service)
    //    {
    //        return await Start(
    //            ControllerContext?.RouteData?.Values["action"]?.ToString(), 
    //            service, 
    //            _userService.Sinhronize(service));
    //    }

    //    private async Task<Model.Result> Start(
    //        string method,
    //        object service,
    //        Task<Service> task)
    //    {
    //        _logger.Log(LogLevel.Information, method);

    //        Model.Result result = new Model.Result();

    //        try
    //        {
    //            int historryId = await AddHsitorry(service, method);

    //            result.Data = JsonConvert.SerializeObject(await task);

    //            await UpdateHistorry(historryId);

    //            result.Status = "Sucsses";

    //            _logger.Log(LogLevel.Information, method + " Sucsses");
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.Log(LogLevel.Error, method + 
    //                $"Fail {ex.InnerException?.Message ?? ex.Message }");

    //            result.Message = ex.InnerException?.Message ?? ex.Message;
    //            result.Status = "Error";
    //        }

    //        return result;
    //    }

    //    private async Task UpdateHistorry(int historryId)
    //    {
    //        var historry = await _applicationDbContext.Historys
    //            .FirstAsync(h => h.Id == historryId);

    //        historry.Result = "Sucsses";

    //        await _applicationDbContext.SaveChangesAsync();
    //    }

    //    private async Task<int> AddHsitorry(object service, string method)
    //    {
    //        var newHistorry = new History()
    //        {
    //            Data = JsonConvert.SerializeObject(service),
    //            CreateAT = DateTime.UtcNow,
    //            UpdateAt = DateTime.UtcNow,
    //            Name = method
    //        };

    //        await _applicationDbContext.Historys.AddAsync(newHistorry);

    //        await _applicationDbContext.SaveChangesAsync();

    //        return newHistorry.Id;
    //    }

    //}
}
