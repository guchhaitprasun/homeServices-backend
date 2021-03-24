using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    /// <summary>
    /// Schema for Services
    /// Author: Prasun Guchhait 
    /// </summary>
    public class ServiceDTO
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public IList<AvailabilityRegionDTO> AvailabilityRegions { get; set; }
    }
}
