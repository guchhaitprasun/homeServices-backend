using MassTransit;
using Microsoft.Extensions.Options;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationServices
{
    public class LogsConsumer : IConsumer<NotificationLogs>
    {
        List<NotificationLogs> logsList;
        public LogsConsumer(IOptions<List<NotificationLogs>> options)
        {
            logsList = options.Value;
        }

        public async Task Consume(ConsumeContext<NotificationLogs> context)
        {
            logsList.Add(context.Message);
        }
    }
}
