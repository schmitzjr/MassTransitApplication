using AutoMapper;
using MassTransitApplication.Services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using mass_transit.Mappings;
using MassTransitApplication.Consumers;

namespace MassTransitApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            services.AddSingleton(config.CreateMapper());
            #endregion
            #region MassTransitProducer
            services.AddMassTransit(trans =>
            {
                trans.UsingRabbitMq((ctx, con) =>
                {
                    con.Host(Configuration.GetConnectionString("AMQPUrl"), cred => 
                    {
                        cred.Username(Configuration.GetConnectionString("AMQPUser"));
                        cred.Password(Configuration.GetConnectionString("AMQPPassword"));
                    });
                });
            });
            #endregion
            #region MassTransitConsumers
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg => 
            {
                cfg.ReceiveEndpoint("greet-new-customers", e => 
                {
                    e.Consumer<CustomerCreatedConsumer>();
                });
            });
            busControl.StartAsync();
            #endregion

            services.AddScoped<ICreateCustomerService, CreateCustomerService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MassTransitApplication", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MassTransitApplication v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}