using Newtonsoft.Json;
using UserStorrage6.Model.Context.Repository;
using UserStorrage6.Model.DB;
using UserStorrage6.Services.Interfaces;

namespace UserStorrage6.Services.Brockers
{
    public class HistorryBrockerService : IHistorryBrockerService
    {
        public async Task UpdateHistorry(IDataBrocker dataBrocker, int historryId)
        {
            var historry = await dataBrocker.GetHistoryByIdAsync(historryId);

            historry.Result = "Sucsses";

            await dataBrocker.SaveChangesAsync();
        }

        public async Task<int> AddHsitorry(IDataBrocker dataBrocker, object service, string method)
        {
            var newHistorry = new History()
            {
                Data = JsonConvert.SerializeObject(service),
                CreateAT = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                Name = method
            };

            dataBrocker.AddHsitorryAsync(newHistorry);

            await dataBrocker.SaveChangesAsync();

            return newHistorry.Id;
        }
    }
}
