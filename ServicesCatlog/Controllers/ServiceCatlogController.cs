using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared;

namespace ServicesCatlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceCatlogController : ControllerBase
    {
        private readonly ILogger<ServiceCatlogController> _logger;
        private List<ServiceDTO> _services = new List<ServiceDTO>();
        private List<AvailabilityRegionDTO> _servicesRegions = new List<AvailabilityRegionDTO>();

        AppMemory memory = new AppMemory();
        public ServiceCatlogController(ILogger<ServiceCatlogController> logger)
        {
            _logger = logger;
            _services = memory.AllServices;
            _servicesRegions = memory.ServiceRegions;
        }

        [HttpGet]
        public string Get()
        {
            return "Catlog Services Working";
        }

        /// <summary>
        /// API to get all the services provided by application
        /// </summary>
        /// <author>Prasun Guchhait</author>
        /// <returns>Returns the list of  service ids and service names wrapped under http response</returns>
        [HttpGet]
        [Route("services")]
        public ActionResult GetService()
        {
            if (_services.Count > 0)
            {
                var allAvailableServices = _services.Select(service => new { service.ServiceID, service.ServiceName }).ToList();
                return Ok(allAvailableServices);
            }
            return BadRequest();  
        }

        /// <summary>
        /// API to get all the Regions where service is available
        /// </summary>
        /// <author>Prasun Guchhait</author>
        /// <returns>Returns the list of regions wrapped under http response</returns>
        [HttpGet]
        [Route("Regions")]
        public ActionResult GetRegions()
        {
            if(_servicesRegions.Count > 0)
            {
                var allRegions = _servicesRegions.Select(region => new { region.RegionID, region.RegionName }).ToList();
                return Ok(allRegions);
            }

            return BadRequest();
            
        }

        /// <summary>
        /// API to get all the regions where the service is available
        /// e.g. will provide all the areas where service Salon For Women is available
        /// </summary>
        /// <author>Prasun Guchhait</author>
        /// <returns>Returns the service id and service name wrapped under hhtp response</returns>
        [HttpGet]
        [Route("{serviceID}")]
        public ActionResult ServiceByServiceID(int serviceID)
        {
            var serviceByServiceID = _services.Where(o => o.ServiceID == serviceID).FirstOrDefault();

            if(serviceByServiceID != null)
            {
                return Ok(serviceByServiceID);
            }

            return BadRequest();
        }

        /// <summary>
        /// API to get all the Services provided in the region
        /// e.g. will provide all the services available in jaipur
        /// </summary>
        /// <param name="regionID">get the services available in region based on region id</param>
        /// <returns>Returns the service id and service name wrapped under hhtp response</returns>
        [HttpGet]
        [Route("Region/{regionID}")]
        public ActionResult ServiceByRegionID(int regionID)
        {
            var serviceByRegionID = this.GetAvailableServices(regionID).Select(region => new { region.ServiceID, region.ServiceName }).ToList();

            if (serviceByRegionID != null)
            {
                return Ok(serviceByRegionID);
            }

            return BadRequest();
        }

        /// <summary>
        /// API to get search a service in availabe in region
        /// </summary>
        /// <param name="regionID"></param>
        /// <param name="serviceID"></param>
        /// <author>Prasun Guchhait</author>
        /// <returns>Return object if service is availabe in that region</returns>
        [HttpGet]
        [Route("Search/{regionID}/{serviceID}")]
        public ActionResult ServiceByTypeAndRegion(int serviceID, int regionID)
        {
            var serviceByServiceID = _services.Where(o => o.ServiceID == serviceID).FirstOrDefault();
            
            if (serviceByServiceID != null)
            {
                var serviceByRegionId = serviceByServiceID.AvailabilityRegions.Where(o => o.RegionID == regionID).FirstOrDefault();

                if(serviceByRegionId != null)
                {
                    return Ok(new { Service = serviceByServiceID.ServiceName, Region = serviceByRegionId });
                }
                else
                {
                    return Ok(new { notAvailable = 1, Message = "Sorry the service is not available in your region " });
                }   
            }

            return BadRequest();
        }

        /// <summary>
        /// gets all available services in region
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        private List<ServiceDTO> GetAvailableServices(int regionId)
        {
            List<ServiceDTO> servicesList = new List<ServiceDTO>();

            foreach (var service in _services)
            {
                foreach (var region in service.AvailabilityRegions)
                {
                    if(region.RegionID == regionId)
                    {
                        ServiceDTO availableService = new ServiceDTO();
                        availableService.ServiceID = service.ServiceID;
                        availableService.ServiceName = service.ServiceName;

                        servicesList.Add(availableService);
                    }

                }
            }

            return servicesList;
        }
    }
}