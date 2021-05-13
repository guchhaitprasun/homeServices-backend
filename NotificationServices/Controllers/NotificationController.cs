using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared;

namespace NotificationServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        List<NotificationLogs> logs;

        public NotificationController(IOptions<List<NotificationLogs>> options)
        {
            logs = options.Value;
        }

        [HttpGet]
        public List<NotificationLogs> Get()
        {
            return logs;
        }
    }
}
