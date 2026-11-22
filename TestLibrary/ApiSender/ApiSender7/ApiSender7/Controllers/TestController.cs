using Company.Shared.Contracts;
using Company.Shared.Messaging.Publishers;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace ApiSender7.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly ICompanyPublisher _publisher;
    public TestController(ISendEndpointProvider sendEndpointProvider, ICompanyPublisher publisher)
    {
        _sendEndpointProvider = sendEndpointProvider;
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

        // 1. التوجيه إلى اسم الطابور المخصص للـ Consumer (وهو TestMessage)
        await _publisher.SendToQueueAsync("notification-processing-queue",
              envelope, null, 10, cancellationToken);


        return Ok(new { Status = "Message sent successfully", TraceId = envelope.CorrelationId });
    }
}