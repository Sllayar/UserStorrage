using UsersStorrage.Models.Context;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using AutoMapper;
using UserStorrage6.Model.Short;
using UserStorrage6.Model.DB;

namespace UserStorrage6.GraphQl.Mutation
{

    [ExtendObjectType("Mutation")]
    public class UserMutation
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<User> GetUsers(
            [Service] ApplicationDbContext applicationDbContext) => 
                applicationDbContext.Users;

        public async Task<User?> AddUser(
            [Service] ApplicationDbContext applicationDbContext,
            [Service] IMapper mapper,
            UserShort user)
        {
            var newUser = mapper.Map<User>(user);

            var service = applicationDbContext.Services.FirstOrDefault(s => s.Key == user.ServiceKey);
            if (service == null) throw new ArgumentException("Сервис с таким ключом не найден.");
            newUser.Service = service;

            var res = await applicationDbContext.AddAsync(newUser);
            await applicationDbContext.SaveChangesAsync();

            return res.Entity;
        }
    }
}
