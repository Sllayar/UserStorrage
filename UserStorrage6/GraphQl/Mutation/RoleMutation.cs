using AutoMapper;
using UsersStorrage.Models.Context;
using UserStorrage6.Model.DB;

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
            RoleRequest roleRequset)
        {
            var newRole = mapper.Map<Role>(roleRequset);

            var service = applicationDbContext.Services.FirstOrDefault(s => s.Key == roleRequset.ServiceKey);
            if (service == null) throw new ArgumentException($"Service с именем {roleRequset.ServiceKey} " +
                    $"не зарегистрирован");
            newRole.Service = service;

            var res = await applicationDbContext.Roles.AddAsync(newRole);
            await applicationDbContext.SaveChangesAsync();

            return res.Entity;
        }
    }
}
