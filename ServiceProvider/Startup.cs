using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jaeger.Senders;
using Jaeger.Senders.Thrift;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Util;
using Shared;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;

namespace ServiceProvider
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
            //singleton to mimic memory
            AppMemory memory = new AppMemory();
            services.AddSingleton<AppMemory>(memory);

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

            services.AddMassTransit(x =>
            {
                x.AddConsumer<OrderConsumer>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.UseHealthCheck(provider);
                    cfg.Host(new Uri($"rabbitmq://{Configuration["RabbitMQHostName"]}"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    cfg.ReceiveEndpoint("ServiceTicketQueue", ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.ConfigureConsumer<OrderConsumer>(provider);
                    });
                }));
            });
            services.AddMassTransitHostedService();
            services.AddControllers();

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
