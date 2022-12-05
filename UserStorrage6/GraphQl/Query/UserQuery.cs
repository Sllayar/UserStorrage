using Microsoft.EntityFrameworkCore;
using UsersStorrage.Models.Context;
using UserStorrage6.Model.DB;

namespace UserStorrage6.GraphQl.Query
{
    [ExtendObjectType("Query")]
    public class UserQuery 
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<User> GetUsers(
            [Service] ApplicationDbContext applicationDbContext) =>
                applicationDbContext.Users;

        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<User> GetUsersPaging(
            [Service] ApplicationDbContext applicationDbContext) =>
                applicationDbContext.Users;

        public async Task<User?> GetUserById(
            [Service] ApplicationDbContext applicationDbContext, int id) =>
                await applicationDbContext.Users.FirstOrDefaultAsync(user => user.Id == id);

        public IQueryable<User> GetUsersById(
            [Service] ApplicationDbContext applicationDbContext, int[] ids) =>
                applicationDbContext.Users.Where(user => ids.Contains(user.Id));
    }
}
