using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceRequest
{
    public class OrderConsumer : IConsumer<OrderDTO>
    {
        AppMemory memory;
        private readonly IConfiguration _configuration;
        private readonly IBus _bus;
        public OrderConsumer(IOptions<AppMemory> options, IConfiguration config, IBus bus)
        {
            memory = options.Value;
            _configuration = config;
            _bus = bus;
        }
        public async Task Consume(ConsumeContext<OrderDTO> context)
        {
            //Notification Service to notify Admin over mail or alert notification
            //To be implemented 

            OrderDTO order = context.Message;
            if(order != null)
            {
                var oldOrder = memory.userData.MyOrders.Where(o => o.ReferenceNumber == order.ReferenceNumber).FirstOrDefault();
                if (oldOrder != null)
                {
                    oldOrder.OrderStatus = order.OrderStatus;
                    oldOrder.StatusID = order.StatusID;
                    oldOrder.ServiceProviderDetails = order.ServiceProviderDetails != null ? order.ServiceProviderDetails : new ServiceProvider();
                }

                bool notification = await SendNotification("Rabbit MQ Consumer >> Order update received for " + order.ReferenceNumber);
            }
        }

        private async Task<bool> SendNotification(string message)
        {
            NotificationLogs logs = new NotificationLogs();
            logs.ServiceName = "Order Services";
            logs.Message = message;


            Uri url = new Uri($"rabbitmq://{_configuration["RabbitMQHostName"]}/NotificationQueue");
            var rabbitService = await _bus.GetSendEndpoint(url);

            await rabbitService.Send(logs);

            return true;
        }
    }
}
