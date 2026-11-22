using Company.Shared.Contracts;
using Company.Shared.Messaging.Publishers;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace LibTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ICompanyPublisher _publisher;
        public WeatherForecastController(ICompanyPublisher publisher)
        {
            _publisher = publisher;
        }  
        [HttpPost("send")]
        public async Task<IActionResult> Send(CancellationToken cancellationToken)
        {
            var envelope = new MessageEnvelope<object>
            {
                EventType = "TestMessageEvent",
                SourceService = "ApiSender",
                CorrelationId = Guid.NewGuid().ToString(),
                Payload = new { Text = "Hello from ApiSender!", SentAt = DateTime.UtcNow }
            };

            // 1. «· ÊÃÌÂ ≈·Ï «”„ «·ÿ«»Ê— «·„Œ’’ ··‹ Consumer (ÊÂÊ TestMessage)
            await _publisher.SendToQueueAsync("notification-orders-queue",
                  envelope, 10, cancellationToken);


            return Ok(new { Status = "Message sent successfully", TraceId = envelope.CorrelationId });
        }
    }
}
