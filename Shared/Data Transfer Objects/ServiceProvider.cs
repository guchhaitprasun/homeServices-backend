using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class ServiceProvider
    {
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }
        public int ServiceTypeID { get; set; }
        public string ServiceName { get; set; }
        public DateTime EstimatedArrivaltime { get; set; }
        public int AmountToBePaid { get; set; }
        public string PhoneNo { get; set; }
    }
}
