using AutoMapper;
using UsersStorrage.Models.Context;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.Short;

namespace UserStorrage6.GraphQl.Mutation
{
    [ExtendObjectType("Mutation")]
    public class ServiceMutation
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Service> GetServices(
           [Service] ApplicationDbContext applicationDbContext) =>
               applicationDbContext.Services;

        public async Task<Service?> AddService(
            [Service] ApplicationDbContext applicationDbContext,
            [Service] IMapper mapper,
            ServiceShort ServiceSyncRequest)
        {
            var newService = mapper.Map<Service>(ServiceSyncRequest);

            var res = await applicationDbContext.Services.AddAsync(newService);
            await applicationDbContext.SaveChangesAsync();

            return res.Entity;
        }
    }
}
