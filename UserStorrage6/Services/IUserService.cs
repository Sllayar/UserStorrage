using UserStorrage6.Model.DB;
using UserStorrage6.Model.Short;

namespace UserStorrage6.Services
{
    public interface IUserService
    {
        Task<Service> Sinhronize(ServiceRequest service);

        Task<Service> PartSinhronize(ServiceRequest service, DateTime currentdate);

        Task<Service> DeleteNotUpdatedUsers(string serviceKey, DateTime currentdate);
    }
}
