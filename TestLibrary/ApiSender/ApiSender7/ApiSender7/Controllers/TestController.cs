using Company.Shared.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace ApiSender7.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public TestController(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send()
    {
        var envelope = new MessageEnvelope<object>
        {
            EventType = "TestMessageEvent",
            SourceService = "ApiSender",
            CorrelationId = Guid.NewGuid().ToString(),
            Payload = new { Text = "Hello from ApiSender!", SentAt = DateTime.UtcNow }
        };

        // 1. التوجيه إلى اسم الطابور المخصص للـ Consumer (وهو TestMessage)
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:TestMessage"));

        // 2. إرسال الرسالة مباشرة إلى هذا الطابور
        await endpoint.Send(envelope);

        return Ok(new { Status = "Message sent successfully", TraceId = envelope.CorrelationId });
    }
}