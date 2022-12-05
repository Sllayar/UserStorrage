using UsersStorrage.Models.Context;
using UserStorrage6.Model.DB;

namespace UserStorrage6.GraphQl.Query
{
    [ExtendObjectType("Query")]
    public class PermissionQuery
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Permission> GetPermission(
            [Service] ApplicationDbContext applicationDbContext) =>
                applicationDbContext.Permissions;

        [UsePaging(IncludeTotalCount = true)]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Permission> GetPermissionPaging(
            [Service] ApplicationDbContext applicationDbContext) =>
                applicationDbContext.Permissions;
    }
}
