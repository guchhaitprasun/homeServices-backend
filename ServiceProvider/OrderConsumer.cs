using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared;

namespace ServiceProvider
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

            OrderDTO receivedOrder = context.Message;
            OrderDTO order = memory.ServiceRequests.Where(o => o.ReferenceNumber == receivedOrder.ReferenceNumber).FirstOrDefault();
            string message = "Rabbit MQ Consumer >> ";
            bool sendNotification = false;
            if (order != null)
            {
                message = message + "Ticket Update Arived " + order.ReferenceNumber;
                order.IsActive = receivedOrder.IsActive;
                sendNotification = true;
            }
            else if (receivedOrder.IsActive)
            {
                message = message + "New Ticket Arived " + receivedOrder.ReferenceNumber;
                memory.ServiceRequests.Add(receivedOrder);
                sendNotification = true;
            }
            //Add Check 
            if (sendNotification)
            {
                bool notifiaction = await SendNotification(message);
            }
        }

        private async Task<bool> SendNotification(string message)
        {
            NotificationLogs logs = new NotificationLogs();
            logs.ServiceName = "Provider Services";
            logs.Message = message;


            Uri url = new Uri($"rabbitmq://{_configuration["RabbitMQHostName"]}/NotificationQueue");
            var rabbitService = await _bus.GetSendEndpoint(url);

            await rabbitService.Send(logs);

            return true;
        }
    }
}
