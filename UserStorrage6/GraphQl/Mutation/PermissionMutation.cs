using AutoMapper;
using UsersStorrage.Models.Context;
using UserStorrage6.Model.DB;

namespace UserStorrage6.GraphQl.Mutation
{
    [ExtendObjectType("Mutation")]
    public class PermissionMutation
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Permission> GetPermissions(
            [Service] ApplicationDbContext applicationDbContext) =>
                applicationDbContext.Permissions;

        public async Task<Permission?> AddPermission(
            [Service] ApplicationDbContext applicationDbContext,
            [Service] IMapper mapper,
            PermissionRequest permissRequset)
        {
            var newPermission = mapper.Map<Permission>(permissRequset);

            var service = applicationDbContext.Services.FirstOrDefault(s => s.Key == permissRequset.ServiceKey);
            if (service == null) throw new ArgumentException($"Service с именем {permissRequset.ServiceKey} " +
                    $"не зарегистрирован");
            newPermission.Service = service;

            var res = await applicationDbContext.Permissions.AddAsync(newPermission);
            await applicationDbContext.SaveChangesAsync();

            return res.Entity;
        }
    }
}
