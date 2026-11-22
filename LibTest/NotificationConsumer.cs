using Company.Shared.Contracts;
using Company.Shared.Messaging.Consumers;
using MassTransit;

namespace LibTest
{
    [QueueName("notification-orders-queue")]
    public sealed class NotificationConsumer : IConsumer 
    {
        public Task Consume(object payload,
            MessageEnvelope<object> envelope)
        {
           
            // Process notification...
            return Task.CompletedTask;
        }
    }
}
