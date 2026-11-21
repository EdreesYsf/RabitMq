using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Shared.Messaging.Options
{
    public class RabbitMqOptions
    {
        public const string SectionName = "RabbitMQ";

        public string Host { get; set; } = "localhost";
        public ushort Port { get; set; } = 5672;
        public string VirtualHost { get; set; } = "/";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";

        public int RetryCount { get; set; } = 3;
        public int RetryIntervalSeconds { get; set; } = 5;

        public string? ServiceQueueName { get; set; }
    }
}
