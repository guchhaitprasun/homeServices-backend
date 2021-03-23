using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class OrderDTO
    {
        public OrderDTO()
        {
            //IsActive = true;
            //Service = new ServiceDTO();
            //Region = new AvailabilityRegionDTO();
            //UserDetails = new UserDTO();
            //ServiceProviderDetails = new ServiceProvider();
            //OrderStatus = "Service Requested";
            //StatusID = 1; //1 for requested 
            //              //2 for awaiting approval 
            //              //3 for Service Request approve
            //              //4 for service Provice successfully
            //              //0 for service Rejected
                            //5 for service cancelled
        }
        public string ReferenceNumber { get; set; }
        public ServiceDTO Service { get; set; }
        public AvailabilityRegionDTO Region { get; set; }
        public UserDTO UserDetails { get; set; }

        public string OrderStatus { get; set; }
        public int StatusID { get; set; }
        public ServiceProvider ServiceProviderDetails { get; set; }

        public bool IsActive { get; set; }
        public bool IsRejected { get; set; }

    }
}
