using System;
using System.Threading.Tasks;
using MassTransitApplication.DTO;
using MassTransitApplication.Events;
using MassTransitApplication.Models;
using MassTransit;
using AutoMapper;

namespace MassTransitApplication.Services
{
    public class CreateCustomerService : ICreateCustomerService
    {
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publisher;

        public CreateCustomerService(IMapper mapper, IPublishEndpoint publisher)
        {
            _mapper = mapper;
            _publisher = publisher;
        }

        public async Task<Customer> Create(CustomerDTO customer)
        {
            var newCustomer = _mapper.Map<Customer>(customer);
            newCustomer.CustomerId = Guid.NewGuid();
            newCustomer.CreatedAt = DateTime.Now;

            await _publisher.Publish<ICustomerCreatedEvent>(newCustomer);

            return newCustomer;
        }
    }
}