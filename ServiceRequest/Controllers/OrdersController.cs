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

namespace ServiceRequest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IBus _bus;
        private readonly IConfiguration _configuration;

        private List<ServiceDTO> _services = new List<ServiceDTO>();
        private List<AvailabilityRegionDTO> _servicesRegions = new List<AvailabilityRegionDTO>();

        AppMemory memory;
        public OrdersController(ILogger<OrdersController> logger, IBus bus, IConfiguration config, IOptions<AppMemory> options)
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
            return "Orders Services Working";
        }

        /// <summary>
        /// Get details of the service to be requested  
        /// </summary>
        /// <param name="serviceID"></param>
        /// <param name="regionID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetService/{regionID}/{serviceID}")]
        public ActionResult GetServiceDetail(int serviceID, int regionID)
        {
            var serviceByServiceID = _services.Where(o => o.ServiceID == serviceID).FirstOrDefault();

            if (serviceByServiceID != null)
            {
                var serviceByRegionId = serviceByServiceID.AvailabilityRegions.Where(o => o.RegionID == regionID).FirstOrDefault();

                if (serviceByRegionId != null)
                {
                    return Ok(new
                    {
                        ServiceID = serviceByServiceID.ServiceID,
                        ServiceName = serviceByServiceID.ServiceName,
                        RegionID = serviceByRegionId.RegionID,
                        RegionName = serviceByRegionId.RegionName,
                        AvailableSlot = serviceByRegionId.NextAvailableDate.Value.ToString("dddd, MMMM dd, yyyy h:mm:ss tt"),
                        ServiceCost = "Rs." + serviceByRegionId.ServiceAmount.ToString()
                    });
                }
                else
                {
                    return Ok(new { notAvailable = 1, Message = "Sorry the service is not available at this moment " });
                }
            }

            return BadRequest();

        }

        /// <summary>
        /// Gets all the Active order and order history 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="authenticationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("MyOrders/{userId}/{authenticationToken}")]
        public ActionResult MyOrders(int userId, string authenticationToken)
        {
            UserDTO user = new UserDTO();
            user.UserId = userId;
            user.AuthenticationToken = authenticationToken;

            List<OrderDTO> orders = new List<OrderDTO>();

            bool isUserAuthenticated = AuthenticateUser(user);

            if (isUserAuthenticated)
            {
                orders = memory.userData.MyOrders;
            }

            return Ok(orders);

        }

        /// <summary>
        /// Gets the order status
        /// </summary>
        /// <param name="orderId">Order id to check order status</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Orderstatus/{orderId}")]
        public ActionResult GerOrderStatus(string orderId)
        {
            OrderDTO orderDetails = memory.userData.MyOrders.Where(o => o.ReferenceNumber == orderId).FirstOrDefault();
            return Ok(new { OrderStatus = orderDetails.OrderStatus });
        }

        /// <summary>
        /// Book a service
        /// </summary>
        /// <param name="serviceID"></param>
        /// <param name="regionID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("BookService/{regionID}/{serviceID}")]
        public async Task<IActionResult> BookService(int serviceID, int regionID, UserDTO user)
        {
            ServiceDTO service = ValidateserviceRequest(serviceID, regionID);
            bool isValidUser = AuthenticateUser(user);

            if (service != null && service.AvailabilityRegions.Count > 0 && isValidUser)
            {
                string serviceid = refernceNumberGenerator();
                OrderDTO order = new OrderDTO();

                order.Service = new ServiceDTO();
                order.UserDetails = new UserDTO();
                order.Region = new AvailabilityRegionDTO();

                order.Service.ServiceID = service.ServiceID;
                order.Service.ServiceName = service.ServiceName;
                order.Region = service.AvailabilityRegions.FirstOrDefault();
                order.UserDetails = user;
                order.ReferenceNumber = serviceid;
                order.StatusID = 1;
                order.OrderStatus = "Service Requested";
                order.IsActive = true;

                //Store entry in Temp List
                memory.userData.MyOrders.Add(order);

                //Call the service bus;
                bool send = await SendServiceRequest(order);
                string message = "Service " + serviceid + " booked by user " + user.FirstName + ' ' + user.LastName;
                if (send)
                {
                    bool res = await SendNotification(message);
                }

                return Ok(new { ReferenceNumber = serviceid });
            }

            return BadRequest();
        }

        [HttpDelete]
        [Route("Cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(string orderId)
        {
            OrderDTO order = memory.userData.MyOrders.Where(o => o.ReferenceNumber == orderId).FirstOrDefault();
            if(order != null && order.IsActive)
            {
                order.IsActive = false;
                order.StatusID = 5;
                order.OrderStatus = memory.GetStatus(5);
                bool send = await SendServiceRequest(order);
                if (send)
                {
                    bool res = await SendNotification("User Cancelled The service order Ref No." + orderId);
                }
                return Ok(new { status = "Order Cancelled Successfully" });
            }

            return Ok(new { status = "Order Already Cancelled or not exist " });

        }


        /// <summary>
        /// validate requested service
        /// </summary>
        /// <param name="serviceID"></param>
        /// <param name="regionID"></param>
        /// <returns></returns>
        private ServiceDTO ValidateserviceRequest(int serviceID, int regionID)
        {
            ServiceDTO service = _services.Where(o => o.ServiceID == serviceID).FirstOrDefault();
            ServiceDTO response;
            if (service != null)
            {
                response = new ServiceDTO();
                response = service;
                response.AvailabilityRegions = service.AvailabilityRegions.Where(region => region.RegionID == regionID).ToList();
            }

            return service;
        }

        /// <summary>
        /// fake authentication 
        /// </summary>
        /// <param name="user">User DTO Received from Paylog of post API</param>
        /// <returns></returns>
        private bool AuthenticateUser(UserDTO user)
        {
            return memory.userData.AuthenticationToken == user.AuthenticationToken && memory.userData.UserId == user.UserId;
        }

        private string refernceNumberGenerator()
        {
            Random _random = new Random();
            const string alphaNums = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(alphaNums, 10).Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        private async Task<bool> SendServiceRequest(OrderDTO order)
        {
            order.UserDetails.AuthenticationToken = string.Empty;
            Uri url = new Uri($"rabbitmq://{_configuration["RabbitMQHostName"]}/CustomerServiceRequestQueue");
            var rabbitService = await _bus.GetSendEndpoint(url);

            await rabbitService.Send(order);
            return true;
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