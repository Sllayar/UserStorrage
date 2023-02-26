using AutoMapper;

using Newtonsoft.Json;

using System.Threading.Tasks;

using UsersStorrage.Models.Context;

using UserStorrage6.Controllers;
using UserStorrage6.Model;
using UserStorrage6.Model.Context.Repository;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.Rest;
using UserStorrage6.Services.Interfaces;

namespace UserStorrage6.Services
{
    public class SynhronizeService<T> : ISynhronizeService<T>
    {
        private readonly ILogger _logger;
        private readonly IHistorryBrockerService _historryService;

        public SynhronizeService(
            ILoggerFactory loggerFactory,
            IHistorryBrockerService historryService)
        {
            _logger = loggerFactory.CreateLogger("SynhronizeService");

            _historryService = historryService;
        }

        public async Task<Result> Synhronize(IDataBrocker dataBrocker, string method, object service, Task<List<T>> data)
        {
            _logger.Log(LogLevel.Information, method);

            Model.Result result = new Model.Result();

            int historryId = 0;

            try
            {
                result.Data = await data;

                historryId = await _historryService.AddHsitorry(dataBrocker, service, method);

                await _historryService.UpdateHistorry(dataBrocker, historryId);

                result.Status = "Sucsses";

                _logger.Log(LogLevel.Information, method + " Sucsses");
            }
            catch (Exception ex)
            {
                if(historryId == 0) await _historryService.AddHsitorry(dataBrocker, service, method);

                _logger.Log(LogLevel.Error, method +
                    $"Fail {ex.InnerException?.Message ?? ex.Message}");

                result.Message = ex.InnerException?.Message ?? ex.Message;
                result.Status = "Error";
            }

            return result;
        }
    }
}
