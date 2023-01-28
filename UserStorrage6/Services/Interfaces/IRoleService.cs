using UserStorrage6.Model.Context.Repository;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.GraphQl;
using UserStorrage6.Model.Requests.Rest;

namespace UserStorrage6.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<Role>?> Synhronize(RoleSyncRequest service, DateTime currentdate);
    }

    public interface IRoleBrockerService
    {
        Task<List<Role>?> Synhronize(IDataBrocker dataBrocker, RoleSyncRequest service, DateTime currentdate);
    }
}
