using UserStorrage6.Controllers;
using UserStorrage6.Model;
using UserStorrage6.Model.Context.Repository;
using UserStorrage6.Model.DB;

namespace UserStorrage6.Services.Interfaces
{
    public interface ISynhronizeService<T>
    {
        //Task<Result> Synhronize(IDataBrocker dataBrocker, string method, object service, Task<List<T>> task);

        Task<Result> Synhronize(IDataBrocker dataBrocker, string method, object service, Task<List<T>> data);
    }
}
