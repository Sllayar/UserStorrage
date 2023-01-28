using AutoMapper;
using UsersStorrage.Models.Context;

using UserStorrage6.Model.Context.Repository;
using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.Rest;
using UserStorrage6.Services.Interfaces;

namespace UserStorrage6.Services.Brockers
{
    public class ServicesBrockerService : IServicesBrockersService
    {
        private readonly IMapper _mapper;

        public ServicesBrockerService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<Service> CreateOrUpdate(IDataBrocker dataBrocker, ServiceSyncRequest service)
        {
            var currentService = dataBrocker.GetServiceByIdAsync(service.Key);

            if (currentService == null) currentService = AddService(dataBrocker, service);
            else UpdateService(service, currentService);

            await dataBrocker.SaveChangesAsync();

            return currentService;
        }

        private Service AddService(IDataBrocker dataBrocker, ServiceSyncRequest service)
        {
            var newService = _mapper.Map<Service>(service);

            dataBrocker.AddServiceAsync(newService);

            return newService;
        }

        private void UpdateService(ServiceSyncRequest service, Service updated)
        {
            updated.Name = service.Name;
            updated.Status = service.Status;
            updated.Author = service.Author;
            updated.Contacts = service.Contacts;
        }
    }
}
