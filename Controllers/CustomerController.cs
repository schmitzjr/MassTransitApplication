using System;
using System.Threading.Tasks;
using MassTransitApplication.DTO;
using MassTransitApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace MassTransitApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICreateCustomerService _service;

        public CustomerController(ICreateCustomerService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerDTO customer)
        {
            try
            {
                var result = await _service.Create(customer);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
    }
}
