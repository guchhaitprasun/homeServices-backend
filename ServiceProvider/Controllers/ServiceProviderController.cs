using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared;

namespace ServiceProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceProviderController : ControllerBase
    {
        private readonly ILogger<ServiceProviderController> _logger;
        private readonly IBus _bus;
        private readonly IConfiguration _configuration;
        AppMemory memory;

        public ServiceProviderController(ILogger<ServiceProviderController> logger, IBus bus, IConfiguration config, IOptions<AppMemory> options)
        {
            memory = options.Value;

            _logger = logger;
            _bus = bus;
            _configuration = config;
        }

        [HttpGet]
        public string Get()
        {
            return "Provider Services Working";
        }

        [HttpGet]
        [Route("GetAllTickets")]
        public ActionResult GetAllTickets()
        {
            var orders = memory.ServiceRequests.Select(ser => new { ser.ReferenceNumber, ser.Region }).ToList();
            return Ok(orders);
        }

        [HttpPut]
        [Route("AcceptService/{orderId}")]
        public async Task<IActionResult> AcceptTicket(string orderId)
        {
            var order = memory.ServiceRequests.Where(o => o.ReferenceNumber == orderId).FirstOrDefault();
            if (order != null && order.IsActive) 
            {
                order.StatusID = 3;
                order.OrderStatus = memory.GetStatus(3);
                bool update = await SendTicketResponse(order);
                if (update)
                {
                    bool res = await SendNotification("Service Request for Order Ref.no " + orderId + " Is Accepted by " + order.ServiceProviderDetails.ProviderName);
                }
                return Ok(order);
            }

            return Ok(new { Message = "Ticket is no longer available" });
            
        }

        [HttpPut]
        [Route("RejectService/{referenceNumber}")]
        public async Task<IActionResult> RejectTicket(string orderId)
        {
            var order = memory.ServiceRequests.Where(o => o.ReferenceNumber == orderId).FirstOrDefault();
            if(order != null && order.IsActive)
            {
                order.StatusID = 0;
                order.OrderStatus = memory.GetStatus(0);
                order.IsRejected = true;
                bool res = await SendTicketResponse(order);
                if (res)
                {
                    bool notification = await SendNotification("Service Request for Order Ref.no " + orderId + " Is Rejected by " + order.ServiceProviderDetails.ProviderName);
                }
            }

            return Ok(new { Message = "Rejected Successfully" });
        }
        private async Task<bool> SendTicketResponse(OrderDTO order)
        {
            bool admin = await InformAdmin(order);
            if (admin)
            {
                bool customer = await InformCustomer(order);

            }

            return true;
        }

        private async Task<bool> InformAdmin(OrderDTO order)
        {
            Uri url = new Uri($"rabbitmq://{_configuration["RabbitMQHostName"]}/ServiceResponseQueue");
            var rabbitService = await _bus.GetSendEndpoint(url);

            await rabbitService.Send(order);
            return true;
        }

        private async Task<bool> InformCustomer(OrderDTO order)
        {
            Uri url = new Uri($"rabbitmq://{_configuration["RabbitMQHostName"]}/CustomerServiceRequestQueue");
            var rabbitService = await _bus.GetSendEndpoint(url);

            await rabbitService.Send(order);
            return true;
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