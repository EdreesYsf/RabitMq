using Company.Shared.Contracts; 
using MassTransit;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace Company.Shared.Messaging.Publishers;

public class CompanyPublisher : ICompanyPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly string _serviceName;

    public CompanyPublisher(
        IPublishEndpoint publishEndpoint,
        ISendEndpointProvider sendEndpointProvider,
        IHostEnvironment environment)
    {
        _publishEndpoint = publishEndpoint;
        _sendEndpointProvider = sendEndpointProvider;
        _serviceName = environment.ApplicationName;
    }

    public async Task PublishAsync<T>(T message, string correlationId = "", byte? priority = null, CancellationToken cancellationToken = default)
        where T : class
    {
        var envelope = MessageEnvelope<T>.Create(message, _serviceName, correlationId);
        //await _publishEndpoint.Publish(envelope, cancellationToken);
        await _publishEndpoint.Publish(envelope, ctx =>
        {
            ApplyPriority(ctx, priority);
        }, cancellationToken);
    }

    public async Task PublishDynamicAsync(
        object dynamicPayload,
        string eventTypeName,
        string correlationId = "", byte? priority = null,
        CancellationToken cancellationToken = default)
    {
        var jsonString = JsonSerializer.Serialize(dynamicPayload);
        using var doc = JsonDocument.Parse(jsonString);

        var envelope = new MessageEnvelope<JsonElement>
        {
            Payload = doc.RootElement.Clone(),
            SourceService = _serviceName,
            EventType = eventTypeName,
            CorrelationId = string.IsNullOrEmpty(correlationId) ? Guid.NewGuid().ToString("N") : correlationId
        };

        await _publishEndpoint.Publish(envelope, ctx =>
        {
            ApplyPriority(ctx, priority);
        }, cancellationToken);
    }
    public async Task SendAsync<T>(
T message,
string exchangeName,
string routingKey,
string correlationId = "",
byte? priority = null,
CancellationToken cancellationToken = default)
where T : class
    {
        var envelope = MessageEnvelope<T>.Create(
        message,
        _serviceName,
        correlationId);
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(
        new Uri($"exchange:{exchangeName}"));
        //await endpoint.Send(envelope, context =>
        //{
        //    context.SetRoutingKey(routingKey);
        //}, cancellationToken);
        await endpoint.Send(envelope, ctx =>
        {
            ctx.SetRoutingKey(routingKey);
            ApplyPriority(ctx, priority);
        }, cancellationToken);
    }
    public async Task SendToQueueAsync<T>(
        string queueName,
        T message,
        string correlationId = "",
        byte? priority = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));
        var envelope = MessageEnvelope<T>.Create(message, _serviceName, correlationId);
        await endpoint.Send(envelope, ctx =>
        {
            ApplyPriority(ctx, priority);
        }, cancellationToken);
    }
    private static void ApplyPriority(SendContext context, byte? priority)
    {
        if (priority.HasValue)
        {
            context.Headers.Set("priority", priority.Value);
        }
    }
}