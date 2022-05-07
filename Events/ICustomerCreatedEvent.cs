using System;
namespace MassTransitApplication.Events
{
    public interface ICustomerCreatedEvent
    {
        Guid CustomerId { get; }
        string Name { get; }
        DateTime BirthDate { get; }
        DateTime CreatedAt { get; }
    }
}