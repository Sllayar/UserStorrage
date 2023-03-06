using UsersStorrage.Models.Context;
using UserStorrage6.Model.DB;

namespace UserStorrage6.GraphQl.Query
{
    [ExtendObjectType("Query")]
    public class RoleQuery
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Role> GetRole(
            [Service] ApplicationDbContext applicationDbContext) =>
                applicationDbContext.Roles;

        [UsePaging(IncludeTotalCount = true, MaxPageSize = 1000)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Role> GetRolePaging(
            [Service] ApplicationDbContext applicationDbContext) =>
                applicationDbContext.Roles;
    }
}
