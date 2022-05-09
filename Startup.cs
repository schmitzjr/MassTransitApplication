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
using MassTransitApplication.Events;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            #region Mapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            services.AddSingleton(config.CreateMapper());
            #endregion

            #region MassTransit configuration
            services.AddMassTransit(trans =>
            {
                trans.AddDelayedMessageScheduler();
                trans.AddConsumer<CustomerCreatedConsumer>();
                trans.AddConsumer<CustomerMailListConsumer>();
                trans.AddRequestClient<ICustomerCreatedEvent>();
                trans.SetKebabCaseEndpointNameFormatter();

                trans.UsingRabbitMq((ctx, con) =>
                {
                    con.Host(Configuration.GetConnectionString("AMQPUrl"), cred => 
                    {
                        cred.Username(Configuration.GetConnectionString("AMQPUser"));
                        cred.Password(Configuration.GetConnectionString("AMQPPassword"));
                    });

                    con.UseDelayedMessageScheduler();

                    var options = new ServiceInstanceOptions()
                        .SetEndpointNameFormatter(ctx.GetService<IEndpointNameFormatter>() ?? KebabCaseEndpointNameFormatter.Instance);
                    
                    con.ServiceInstance(options, instance =>
                    {
                        instance.ConfigureEndpoints(ctx);
                    });    
                });
            });
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
            }

            app.UseSwagger();
            
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MassTransitApplication v1"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("ready"),
                    ResponseWriter = HealthCheckResponseWriter
                });

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions {ResponseWriter = HealthCheckResponseWriter});
                endpoints.MapControllers();
            });
        }

        static Task HealthCheckResponseWriter(HttpContext context, HealthReport result)
        {
            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(entry => new JProperty(entry.Key, new JObject(
                    new JProperty("status", entry.Value.Status.ToString()),
                    new JProperty("description", entry.Value.Description),
                    new JProperty("data", JObject.FromObject(entry.Value.Data))))))));

            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(json.ToString(Formatting.Indented));
        }
    }
}