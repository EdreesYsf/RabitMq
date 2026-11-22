using Company.Shared.Contracts;
using Company.Shared.Messaging.Consumers;
using MassTransit;
using MassTransit.SqlTransport;
using System.Text.Json;

namespace LibTest
{
    [QueueName("notification-orders-queue")] 
    public class NotificationConsumer : BaseCompanyConsumer<object>
    { 
        private readonly ILogger<NotificationConsumer> _logger; 

        public NotificationConsumer( 
            ILogger<NotificationConsumer> logger)
            : base(logger)
        { 
            _logger = logger; 
        }


        // تطبيق الدالة الإلزامية لمعالجة محتوى الرسالة
        protected override async Task ProcessMessageAsync(
            object payload,
            MessageEnvelope<object> envelope,
            ConsumeContext<MessageEnvelope<object>> context)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        
        }
    }
}
