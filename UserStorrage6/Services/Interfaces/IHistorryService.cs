using UserStorrage6.Model.Context.Repository;

namespace UserStorrage6.Services.Interfaces
{
    public interface IHistorryBrockerService
    {
        Task UpdateHistorry(IDataBrocker dataBrocker, int historryId);

        Task<int> AddHsitorry(IDataBrocker dataBrocker, object service, string method);
    }

    public interface IHistorryService
    {
        Task UpdateHistorry(int historryId);

        Task<int> AddHsitorry(object service, string method);
    }
}
