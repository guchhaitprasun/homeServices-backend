using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using Shared;
using MassTransit;
using OpenTracing;
using Jaeger.Senders;
using Jaeger.Senders.Thrift;
using OpenTracing.Util;

namespace ServiceRequest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //Add CORS Policy
            services.AddCors(op =>
            {
                op.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddSingleton<ITracer>(serviceProvider =>
            {
                ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                Jaeger.Configuration.SenderConfiguration.DefaultSenderResolver = new SenderResolver(loggerFactory).RegisterSenderFactory<ThriftSenderFactory>();

                var config = Jaeger.Configuration.FromEnv(loggerFactory);
                ITracer tracer = config.GetTracer();
                GlobalTracer.Register(tracer);

                return tracer;
            });

            services.AddOpenTracing();

            //Service bus configuration
            services.AddMassTransit(mt =>
            {
                mt.AddConsumer<OrderConsumer>();
                mt.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(confg =>
                {
                    confg.UseHealthCheck(provider);
                    confg.Host(new Uri($"rabbitmq://{Configuration["RabbitMQHostName"]}"), host =>
                    {
                        host.Username("guest");
                        host.Password("guest");
                    });

                    confg.ReceiveEndpoint("ServiceResponseQueue", ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.ConfigureConsumer<OrderConsumer>(provider);
                    });
                }));
            });

            services.AddMassTransitHostedService();

            //Controller Addition    
            services.AddControllers();

            //singleton to mimic memory
            AppMemory memory = new AppMemory();
            services.AddSingleton<AppMemory>(memory);

            //Service Discovery and Registry 
            services.AddDiscoveryClient(Configuration);
            services.AddHealthChecks();
            services.AddSingleton<IHealthCheckHandler, ScopedEurekaHealthCheckHandler>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //discovery 
            app.UseDiscoveryClient();
            app.UseHealthChecks("/info");
        }
    }
}
