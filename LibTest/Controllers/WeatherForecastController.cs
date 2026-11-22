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
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
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
            await _publisher.SendToQueueAsync("notification-processing-queue",
                  envelope, null, 10, cancellationToken);


            return Ok(new { Status = "Message sent successfully", TraceId = envelope.CorrelationId });
        }
    }
}
