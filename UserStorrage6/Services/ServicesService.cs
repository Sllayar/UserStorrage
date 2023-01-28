using AutoMapper;

using Microsoft.EntityFrameworkCore;

using UsersStorrage.Models.Context;

using UserStorrage6.Model.DB;
using UserStorrage6.Model.Requests.Rest;
using UserStorrage6.Services.Interfaces;

namespace UserStorrage6.Services
{
    public class ServicesService : IServicesService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public ServicesService(
            ApplicationDbContext applicationDbContext,
            IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<Service> CreateOrUpdate(ServiceSyncRequest service)
        {
            var currentService =
                _applicationDbContext.Services
                .FirstOrDefault(dbService => service.Key == dbService.Key);

            if (currentService == null) currentService = AddService(service);
            else UpdateService(service, currentService);

            await _applicationDbContext.SaveChangesAsync();

            return currentService;
        }

        private Service AddService(ServiceSyncRequest service)
        {
            var newService = _mapper.Map<Service>(service);

            _applicationDbContext.Services.Add(newService);

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
