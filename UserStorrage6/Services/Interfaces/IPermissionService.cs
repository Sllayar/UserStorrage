using UserStorrage6.Model.Context.Repository;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.GraphQl;
using UserStorrage6.Model.Requests.Rest;
using UserStorrage6.Model.Requests.Short;

namespace UserStorrage6.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<List<Permission>?> Synhronize(ServiceSyncPermissionRequest service, DateTime currentdate);

        //Task<List<Permission>?> Synhronize(List<PermissionShort>? Permitions, Service service, DateTime currentdate, DateTime syncTime);

        //Task<List<Permission>?> Synhronize(RoleSyncRequest roleSyncRequest, DateTime currentDate, DateTime syncTime);

        Permission TryUpdatePermission(Permission permission, PermissionShort newPermission, DateTime currentdate, DateTime syncTime);

        Permission CreatePermission(Service serviceKey, PermissionShort newPermission, DateTime currentdate, DateTime syncTime);

        void DeleteNotUpdatedPermissions(List<Permission>? permissions, List<string>? exceptPermissions, DateTime currentdate, DateTime syncTime);
    }

    public interface IPermissionBrockerService
    {
        Task<List<Permission>?> Synhronize(IDataBrocker dataBrocker, ServiceSyncPermissionRequest service, DateTime currentdate);

        Task<List<Permission>?> Synhronize(IDataBrocker dataBrocker, List<PermissionShort>? Permitions, Service service, DateTime currentdate, DateTime syncTime);

        Task<List<Permission>?> SynhronizePart(IDataBrocker dataBrocker, ServiceSyncPermissionRequest service, DateTime currentdate);

        Task<List<Permission>?> SynhronizePartFinish(IDataBrocker dataBrocker, string serviceKey, DateTime syncDate);
    }
}
