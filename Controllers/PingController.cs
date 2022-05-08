using System;
using System.Threading.Tasks;
using MassTransitApplication.DTO;
using MassTransitApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace MassTransitApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        public PingController()
        {
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Pong");                
        }
    }
}
