using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransitApplication.Events;

namespace MassTransitApplication.Consumers
{
    public class CustomerMailListConsumer : IConsumer<ICustomerCreatedEvent>
    {
        public Task Consume(ConsumeContext<ICustomerCreatedEvent> context)
        {
            Console.WriteLine($"New customer added to mail list {context.Message.CustomerId.ToString()} - {context.Message.Name}");
            
            return Task.CompletedTask;
        }
    }
}