using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class UserDTO
    {
        public int UserId { get; set; }

        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string AuthenticationToken { get; set; }

        public List<OrderDTO> MyOrders { get; set; }

    }
}
