using UserStorrage6.Model.Context.Repository;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.Rest;

namespace UserStorrage6.Services.Interfaces
{
    public interface IUserService
    {
        Task<Service> Sinhronize(ServiceSyncUserRequest service);

        Task<Service> PartSinhronize(ServiceSyncUserRequest service, DateTime currentdate);

        Task<Service> DeleteNotUpdatedUsers(string serviceKey, DateTime currentdate);
    }

    public interface IUserBrockerService
    {
        Task<List<User>?> Sinhronize(IDataBrocker dataBrocker, ServiceSyncUserRequest service);

        Task<List<User>?> PartSinhronize(IDataBrocker dataBrocker, ServiceSyncUserRequest service, DateTime currentdate);

        Task<List<User>?> DeleteNotUpdatedUsers(IDataBrocker dataBrocker, string serviceKey, DateTime currentdate);
    }
}
