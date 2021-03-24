using System;
using System.Collections.Generic;

namespace Shared
{
    /// <summary>
    /// Static Memory act as a In Memory Data (Temp data) for this Application 
    /// Used to fetch and store dummy
    /// Author: Prasun Guchhait 
    /// </summary>
    public class AppMemory
    {
        /// Constructor to initialise memory with dummy data 
        public AppMemory()
        {
            AllServices = _Services;
            ServiceRegions = _Regions;
            userData = _dummyUser;
            ServiceRequests = new List<OrderDTO>();
            ServiceProviders = _ServiceProviders;
        }

        /// List of Services Available 
        public List<ServiceDTO> AllServices { get; set; }
        
        //Property to hold Regions of operations
        public List<AvailabilityRegionDTO> ServiceRegions { get; set; }

        public UserDTO userData { get; set; }

        public List<OrderDTO> ServiceRequests { get; set; }

        public  List<ServiceProvider> ServiceProviders { get; set; }

        public string GetStatus(int statusId)
        {
            switch (statusId)
            {
                case 1:
                    return "Service Requested";
                case 2:
                    return "Awaiting Approval";
                case 3:
                    return "Service Approve";
                case 4:
                    return "Order Deliverd Successfully";
                case 5:
                    return "Order Cancelled";
                default:
                    return "Service Rejected";
            }

        }

        /// <summary>
        /// to mimic db call to fetch regions data  
        /// Author: Prasun Guchhait 
        /// </summary>
        private static readonly List<AvailabilityRegionDTO> _Regions = new List<AvailabilityRegionDTO>()
        {
            new AvailabilityRegionDTO(){ RegionID = 1, RegionName = "Jaipur"}, 
            new AvailabilityRegionDTO(){ RegionID = 2, RegionName = "Delhi"}, 
            new AvailabilityRegionDTO(){ RegionID = 3, RegionName = "Gurugram"}, 
            new AvailabilityRegionDTO(){ RegionID = 4, RegionName = "Noida"}, 
            new AvailabilityRegionDTO(){ RegionID = 5, RegionName = "Goa"}, 
            new AvailabilityRegionDTO(){ RegionID = 6, RegionName = "Kolkata"}, 
            new AvailabilityRegionDTO(){ RegionID = 7, RegionName = "Chennai"}, 
            new AvailabilityRegionDTO(){ RegionID = 8, RegionName = "Hyderabad"}, 
            new AvailabilityRegionDTO(){ RegionID = 9, RegionName = "Bengaluru"}, 
            new AvailabilityRegionDTO(){ RegionID = 10, RegionName = "Pune"}, 
            new AvailabilityRegionDTO(){ RegionID = 11, RegionName = "Mumbai"}

        };
        private static readonly List<ServiceDTO> _Services = new List<ServiceDTO>()
        {
            new ServiceDTO(){
                ServiceID = 20210,
                ServiceName = "Salon For Women",
                AvailabilityRegions = new List<AvailabilityRegionDTO>()
                {
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 1,
                        RegionName = "Jaipur",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                         RegionID = 8,
                        RegionName = "Hyderabad",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                         RegionID = 7,
                        RegionName = "Chennai",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                     new AvailabilityRegionDTO()
                    {
                          RegionID = 10,
                        RegionName = "Pune",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                      new AvailabilityRegionDTO()
                    {
                           RegionID = 11,
                        RegionName = "Mumbai",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    }
                }
            },
            new ServiceDTO(){
                ServiceID = 20211,
                ServiceName = "Massage For Women",
                AvailabilityRegions = new List<AvailabilityRegionDTO>()
                {
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 1,
                        RegionName = "Jaipur",
                        ServiceAmount = 5000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 7,
                        RegionName = "Chennai",
                        ServiceAmount = 5000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 2,
                        RegionName = "Delhi",
                        ServiceAmount = 5000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                     new AvailabilityRegionDTO()
                    {
                        RegionID = 10,
                        RegionName = "Pune",
                        ServiceAmount = 5000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                      new AvailabilityRegionDTO()
                    {
                        RegionID = 6,
                        RegionName = "Kolkata",
                        ServiceAmount = 5000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    }
                }
            },
            new ServiceDTO(){
                ServiceID = 20212,
                ServiceName = "Salon For Men",
                AvailabilityRegions = new List<AvailabilityRegionDTO>()
                {
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 1,
                        RegionName = "Jaipur",
                        ServiceAmount = 200,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 2,
                        RegionName = "Delhi",
                        ServiceAmount = 200,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 3,
                        RegionName = "Gurugram",
                        ServiceAmount = 200,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                     new AvailabilityRegionDTO()
                    {
                        RegionID = 4,
                        RegionName = "Noida",
                        ServiceAmount = 200,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                      new AvailabilityRegionDTO()
                    {
                        RegionID = 5,
                        RegionName = "Goa",
                        ServiceAmount = 200,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    }
                }
            },
            new ServiceDTO(){
                ServiceID = 20213,
                ServiceName = "Massage For Men",
                AvailabilityRegions = new List<AvailabilityRegionDTO>()
                {
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 1,
                        RegionName = "Jaipur",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 3,
                        RegionName = "Gurugram",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 6,
                        RegionName = "Kolkata",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 4,
                        RegionName = "Noida",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 5,
                        RegionName = "Goa",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    }
                }
            },
            new ServiceDTO(){
                ServiceID = 20214,
                ServiceName = "AC Servic & Repair",
                AvailabilityRegions = new List<AvailabilityRegionDTO>()
                {
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 1,
                        RegionName = "Jaipur",
                        ServiceAmount = 1200,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 4,
                        RegionName = "Noida",
                        ServiceAmount = 1200,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 5,
                        RegionName = "Goa",
                        ServiceAmount = 1200,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                     new AvailabilityRegionDTO()
                    {
                         RegionID = 6,
                        RegionName = "Kolkata",
                        ServiceAmount = 1200,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                      new AvailabilityRegionDTO()
                    {
                          RegionID = 7,
                        RegionName = "Chennai",
                        ServiceAmount = 1200,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    }
                }
            },
            new ServiceDTO(){
                ServiceID = 20215,
                ServiceName = "Appliance Repair",
                AvailabilityRegions = new List<AvailabilityRegionDTO>()
                {
                    new AvailabilityRegionDTO()
                    {
                          RegionID = 1,
                        RegionName = "Jaipur",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                          RegionID = 5,
                        RegionName = "Goa",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                          RegionID = 6,
                        RegionName = "Kolkata",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                     new AvailabilityRegionDTO()
                    {
                           RegionID = 7,
                        RegionName = "Chennai",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                      new AvailabilityRegionDTO()
                    {
                            RegionID = 8,
                        RegionName = "Hyderabad",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    }
                }
            },
            new ServiceDTO(){
                ServiceID = 20216,
                ServiceName = "Electrician",
                AvailabilityRegions = new List<AvailabilityRegionDTO>()
                {
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 1,
                        RegionName = "Jaipur",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 6,
                        RegionName = "Kolkata",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 7,
                        RegionName = "Chennai",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                     new AvailabilityRegionDTO()
                    {
                        RegionID = 8,
                        RegionName = "Hyderabad",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                      new AvailabilityRegionDTO()
                    {
                        RegionID = 9,
                        RegionName = "Bengaluru",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    }
                }
            },
            new ServiceDTO(){
                ServiceID = 20217,
                ServiceName = "Plumber",
                AvailabilityRegions = new List<AvailabilityRegionDTO>()
                {
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 1,
                        RegionName = "Jaipur",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 7,
                        RegionName = "Chennai",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 8,
                        RegionName = "Hyderabad",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                     new AvailabilityRegionDTO()
                    {
                        RegionID = 9,
                        RegionName = "Bengaluru",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                      new AvailabilityRegionDTO()
                    {
                        RegionID = 11,
                        RegionName = "Mumbai",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    }
                }
            },
            new ServiceDTO(){
                ServiceID = 20218,
                ServiceName = "Carpenter",
                AvailabilityRegions = new List<AvailabilityRegionDTO>()
                {
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 1,
                        RegionName = "Jaipur",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 8,
                        RegionName = "Hyderabad",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 7,
                        RegionName = "Chennai",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                     new AvailabilityRegionDTO()
                    {
                        RegionID = 4,
                        RegionName = "Noida",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                      new AvailabilityRegionDTO()
                    {
                        RegionID = 11,
                        RegionName = "Mumbai",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    }
                }
            },
            new ServiceDTO(){
                ServiceID = 20219,
                ServiceName = "Pest Control",
                AvailabilityRegions = new List<AvailabilityRegionDTO>()
                {
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 1,
                        RegionName = "Jaipur",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 8,
                        RegionName = "Hyderabad",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                    new AvailabilityRegionDTO()
                    {
                        RegionID = 7,
                        RegionName = "Chennai",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                     new AvailabilityRegionDTO()
                    {
                        RegionID = 10,
                        RegionName = "Pune",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    },
                      new AvailabilityRegionDTO()
                    {
                        RegionID = 11,
                        RegionName = "Mumbai",
                        ServiceAmount = 2000,
                        NextAvailableDate = DateTime.Now.AddDays(1)
                    }
                }
            }
        };
        private static readonly UserDTO _dummyUser = new UserDTO()
        {
            UserId = 123456789,
            EmailAddress = "banana@yello.in",
            FirstName = "Yello",
            LastName = "Banana",
            Address = "Available Throughout India",
            AuthenticationToken = "babanana",
            MyOrders = new List<OrderDTO>()
        };
        private static readonly List<ServiceProvider> _ServiceProviders = new List<ServiceProvider>()
        {
            new ServiceProvider()
            {
                ProviderId = 202103011,
                ProviderName = "Hema",
                ServiceTypeID = 20210,
                ServiceName = "Salon For Women",
                EstimatedArrivaltime = DateTime.Now,
                AmountToBePaid = 2000,
                PhoneNo = "1111111111"
            },
            new ServiceProvider()
            {
                ProviderId = 202103012,
                ProviderName = "Rekha",
                ServiceTypeID = 20211,
                ServiceName = "Massage For Women",
                EstimatedArrivaltime = DateTime.Now,
                AmountToBePaid = 5000,
                PhoneNo = "2222222222"
            },
            new ServiceProvider()
            {
                ProviderId = 202103013,
                ProviderName = "Jaya",
                ServiceTypeID = 20212,
                ServiceName = "Salon For Men",
                EstimatedArrivaltime = DateTime.Now,
                AmountToBePaid = 200,
                PhoneNo = "2222222222"
            },
            new ServiceProvider()
            {
                ProviderId = 202103014,
                ProviderName = "Sushma",
                ServiceTypeID = 20213,
                ServiceName = "Massage For Men",
                EstimatedArrivaltime = DateTime.Now,
                AmountToBePaid = 1200,
                PhoneNo = "2222222222"
            }
        };
    }

}
