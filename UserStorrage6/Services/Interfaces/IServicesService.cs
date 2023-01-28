using UserStorrage6.Model.Context.Repository;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.Rest;

namespace UserStorrage6.Services.Interfaces
{
    public interface IServicesBrockersService
    {
        Task<Service> CreateOrUpdate(IDataBrocker dataBrocker, ServiceSyncRequest service);
    }
    public interface IServicesService
    {
        Task<Service> CreateOrUpdate(ServiceSyncRequest service);
    }
}
