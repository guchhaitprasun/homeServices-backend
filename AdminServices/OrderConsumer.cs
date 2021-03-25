using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared;

namespace AdminServices
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
            bool IsServiceCancellRequest = false;
            string message = "Rabbit MQ Consumer >> ";
            if (order != null)
            {
                message = message + "Order Update Received " + order.ReferenceNumber;
                IsServiceCancellRequest = order.IsActive && !receivedOrder.IsActive;

                order.IsActive = receivedOrder.IsActive;
                order.IsRejected = receivedOrder.IsRejected;
                order.OrderStatus = receivedOrder.OrderStatus;
                order.StatusID = receivedOrder.StatusID;
            }
            else
            {
                message = message + "New Order Received " + receivedOrder.ReferenceNumber;
                memory.ServiceRequests.Add(receivedOrder);
            }

            if (IsServiceCancellRequest)
            {
                Uri url = new Uri($"rabbitmq://{_configuration["RabbitMQHostName"]}/ServiceTicketQueue");
                var rabbitService = await _bus.GetSendEndpoint(url);

                await rabbitService.Send(order);
            }
            bool notification = await SendNotification(message);
        }

        private async Task<bool> SendNotification(string message)
        {
            NotificationLogs logs = new NotificationLogs();
            logs.ServiceName = "Admin Services";
            logs.Message = message;


            Uri url = new Uri($"rabbitmq://{_configuration["RabbitMQHostName"]}/NotificationQueue");
            var rabbitService = await _bus.GetSendEndpoint(url);

            await rabbitService.Send(logs);

            return true;
        }
    }
}
