using System.Threading.Tasks;
using MassTransitApplication.DTO;
using MassTransitApplication.Models;

namespace MassTransitApplication.Services
{
    public interface ICreateCustomerService
    {
        Task<Customer> Create(CustomerDTO customer);
    }
}