using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransitApplication.Events;

namespace MassTransitApplication.Consumers
{
    public class CustomerCreatedConsumer : IConsumer<ICustomerCreatedEvent>
    {
        public Task Consume(ConsumeContext<ICustomerCreatedEvent> context)
        {
            Console.WriteLine($"New customer added {context.Message.CustomerId.ToString()} - {context.Message.Name}");
            
            return Task.CompletedTask;
        }
    }
}