using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;
using UsersStorrage.Models.Context;
using UserStorrage6.Model.DB;
using UserStorrage6.Services.Interfaces;

namespace UserStorrage6.Services
{
    public class HistorryService : IHistorryService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public HistorryService(
            ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task UpdateHistorry(int historryId)
        {
            var historry = await _applicationDbContext.Historys
                .FirstAsync(h => h.Id == historryId);

            historry.Result = "Sucsses";

            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<int> AddHsitorry(object service, string method)
        {
            var newHistorry = new History()
            {
                Data = JsonConvert.SerializeObject(service),
                CreateAT = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                Name = method
            };

            await _applicationDbContext.Historys.AddAsync(newHistorry);

            await _applicationDbContext.SaveChangesAsync();

            return newHistorry.Id;
        }
    }
}
