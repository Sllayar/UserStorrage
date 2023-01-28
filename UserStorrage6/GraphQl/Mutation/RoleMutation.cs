using AutoMapper;
using UsersStorrage.Models.Context;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.Short;

namespace UserStorrage6.GraphQl.Mutation
{
    [ExtendObjectType("Mutation")]
    public class RoleMutation
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Role> GetRoles(
            [Service] ApplicationDbContext applicationDbContext) =>
                applicationDbContext.Roles;

        public async Task<Role?> AddRole(
            [Service] ApplicationDbContext applicationDbContext,
            [Service] IMapper mapper,
            RoleShort roleRequset)
        {
            var newRole = mapper.Map<Role>(roleRequset);

            var service = applicationDbContext.Services.FirstOrDefault(s => s.Key == roleRequset.SysId);
            if (service == null) throw new ArgumentException($"Service с именем {roleRequset.SysId} " +
                    $"не зарегистрирован");
            newRole.Service = service;

            var res = await applicationDbContext.Roles.AddAsync(newRole);
            await applicationDbContext.SaveChangesAsync();

            return res.Entity;
        }
    }
}
