using UsersStorrage.Models.Context;
using UserStorrage6.Model.DB;

namespace UserStorrage6.GraphQl.Query
{
    [ExtendObjectType("Query")]
    public class ServicesQuery
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Service> GetServices(
            [Service] ApplicationDbContext applicationDbContext) =>
                applicationDbContext.Services;

        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Service> GetServicesPaging(
            [Service] ApplicationDbContext applicationDbContext) =>
                applicationDbContext.Services;
    }
}
