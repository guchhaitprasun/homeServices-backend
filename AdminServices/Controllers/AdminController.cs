using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminServices.Controllers
{
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IBus _bus;
        private readonly IConfiguration _configuration;

        private List<ServiceDTO> _services = new List<ServiceDTO>();
        private List<AvailabilityRegionDTO> _servicesRegions = new List<AvailabilityRegionDTO>();

        AppMemory memory;
        public AdminController(ILogger<AdminController> logger, IBus bus, IConfiguration config,IOptions<AppMemory> options)
        {
            memory = options.Value;

            _logger = logger;
            _bus = bus;
            _configuration = config;
            _services = memory.AllServices;
            _servicesRegions = memory.ServiceRegions;
        }
        [HttpGet]
        public string Get()
        {
            return "Admin Services Working";
        }

        [HttpGet]
        [Route("ServiceRequests/GetNewRequests")]
        public IList<OrderDTO> GetNewServiceRequests()
        {
            List<OrderDTO> orders = memory.ServiceRequests.Where(o => o.StatusID == 1 && o.IsActive).ToList();
            return orders;
        }

        [HttpGet]
        [Route("ServiceRequests/{orderId}")]
        public ActionResult GetOrderByServiceId(string serviceId)
        {
            OrderDTO order = memory.ServiceRequests.Where(o => o.ReferenceNumber == serviceId).FirstOrDefault();
            if(order != null)
            {
                return Ok(order);
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("UpdateRequestState/{orderId}/{statusId}")]
        public async Task<IActionResult> UpdateServiceStatus(string orderId, int statusId)
        {
            OrderDTO orderDto = memory.ServiceRequests.Where(o => o.ReferenceNumber == orderId).FirstOrDefault();
            if(orderDto != null && orderDto.IsActive)
            {
                orderDto.StatusID = statusId;
                orderDto.OrderStatus = memory.GetStatus(statusId);

                bool sendUpdate = await UpdateOrderStatus(orderDto);
                if (sendUpdate)
                {
                    bool res = await SendNotification("Status of Order Ref.No " + orderId + " Is updated to " + orderDto.OrderStatus);
                }
                return Ok(new { message = "Status of Order Ref.No " + orderId + " Is updated to " + orderDto.OrderStatus });
            }

            return Ok(new { message = "Order Is Cancelled or not exist" });
        }

        [HttpGet]
        [Route("GetAvailableProviders/{orderId}")]
        public ActionResult GetProviderForService(string orderId)
        {
            OrderDTO order = memory.ServiceRequests.Where(o => o.ReferenceNumber == orderId).FirstOrDefault();
            if(order != null)
            {
                if (order.IsActive)
                {
                    var provider = memory.ServiceProviders.Where(o => o.ServiceTypeID == order.Service.ServiceID).FirstOrDefault();
                    if(provider != null)
                    {
                        return Ok(provider);
                    }
                    else
                    {
                        return Ok(new { Message = "No Provider availabe at the moment" });
                    }
                }
                else
                {
                    return Ok( new { Message = "Order is cancelled"});
                }
            }
            return BadRequest();
        }

        [HttpPut]
        [Route("AssignOrderToProvider/{orderId}/{providerId}")]
        public async Task<IActionResult> AssignOrder(string orderId, int providerId)
        {
            ServiceProvider provider = memory.ServiceProviders.Where(o => o.ProviderId == providerId).FirstOrDefault();
            OrderDTO order = memory.ServiceRequests.Where(o => o.ReferenceNumber == orderId).FirstOrDefault();
            if(provider != null)
            {
                if(order != null)
                {
                    string responseMessage = "Service request is assigned  to " + provider.ProviderName + "(" + provider.ProviderId.ToString() + ")";
                    order.ServiceProviderDetails = provider;
                    bool response = await AssignTicket(order);
                    if (response)
                    {
                        bool res = await SendNotification("Order Ref.no " + orderId + " is Assigned to " + provider.ProviderName);
                    }
                    return Ok(new { Message = responseMessage});                    
                }
                return Ok(new { Message = "Order is eithe cancelled or not exist" });
            }

            return Ok(new { message = "Selected provider is not availabel" });
        }

        private async Task<bool> UpdateOrderStatus(OrderDTO order)
        {
            Uri url = new Uri($"rabbitmq://{_configuration["RabbitMQHostName"]}/ServiceResponseQueue");
            var rabbitService = await _bus.GetSendEndpoint(url);

            await rabbitService.Send(order);
            return true;
        }

        private async Task<bool> AssignTicket(OrderDTO order)
        {
            Uri url = new Uri($"rabbitmq://{_configuration["RabbitMQHostName"]}/ServiceTicketQueue");
            var rabbitService = await _bus.GetSendEndpoint(url);

            await rabbitService.Send(order);
            return true;
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
