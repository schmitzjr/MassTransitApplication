using AutoMapper;
using MassTransitApplication.DTO;
using MassTransitApplication.Models;

namespace mass_transit.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CustomerDTO, Customer>();
        }
    }
}