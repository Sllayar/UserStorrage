using UserStorrage6.Model.DB;
using UserStorrage6.Model.Short;

namespace UserStorrage6.Services
{
    public interface IUserService
    {
        Task<Service> Sinhronize(ServiceRequest service);
    }
}
