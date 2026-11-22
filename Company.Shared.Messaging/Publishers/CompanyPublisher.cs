using System.Text.Json;
using Company.Shared.Contracts;
using Company.Shared.Messaging.Configuration;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Hosting;

namespace Company.Shared.Messaging.Publishers;
 

public class CompanyPublisher : ICompanyPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly MessagingOptions _options;
    private readonly string _serviceName;

    public CompanyPublisher(
        IPublishEndpoint publishEndpoint,
        ISendEndpointProvider sendEndpointProvider,
        IHostEnvironment environment,
        MessagingOptions options)
    {
        _publishEndpoint = publishEndpoint;
        _sendEndpointProvider = sendEndpointProvider;
        _options = options;
        _serviceName = environment.ApplicationName;
    }

    public async Task PublishAsync<T>(
        MessageEnvelope<T> envelope,
        byte? priority = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        NormalizeEnvelope(envelope);

        await _publishEndpoint.Publish(envelope, ctx =>
        {
            ConfigureSendContext(ctx, envelope, priority, _options.MaxPriority);
        }, cancellationToken);
    }

    public async Task PublishDynamicAsync(
        MessageEnvelope<JsonElement> envelope,
        byte? priority = null,
        CancellationToken cancellationToken = default)
    {
        NormalizeEnvelope(envelope);

        await _publishEndpoint.Publish(envelope, ctx =>
        {
            ConfigureSendContext(ctx, envelope, priority, _options.MaxPriority);
        }, cancellationToken);
    }

    public async Task SendAsync<T>(
        MessageEnvelope<T> envelope,
        string exchangeName,
        string routingKey,
        byte? priority = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        NormalizeEnvelope(envelope);
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:{exchangeName}"));

        await endpoint.Send(envelope, ctx =>
        {
            ctx.SetRoutingKey(routingKey);
            ConfigureSendContext(ctx, envelope, priority, _options.MaxPriority);
        }, cancellationToken);
    }

    public async Task SendToQueueAsync<T>(
        string queueName,
        MessageEnvelope<T> envelope,
        byte? priority = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        NormalizeEnvelope(envelope);
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}?bind=false"));

        await endpoint.Send(envelope, ctx =>
        {
            ConfigureSendContext(ctx, envelope, priority, _options.MaxPriority);
        }, cancellationToken);
    }

    private MessageEnvelope<T> NormalizeEnvelope<T>(MessageEnvelope<T> envelope)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        var sourceService = string.IsNullOrWhiteSpace(envelope.SourceService)
            ? _serviceName
            : envelope.SourceService;

        var correlationId = string.IsNullOrWhiteSpace(envelope.CorrelationId)
            ? Guid.NewGuid().ToString("N")
            : envelope.CorrelationId;

        if (sourceService == envelope.SourceService && correlationId == envelope.CorrelationId)
        {
            return envelope;
        }

        return new MessageEnvelope<T>
        {
            MessageId = envelope.MessageId,
            CorrelationId = correlationId,
            SourceService = sourceService,
            Timestamp = envelope.Timestamp,
            EventType = envelope.EventType,
            Payload = envelope.Payload
        };
    }

    private static void ConfigureSendContext<T>(
        SendContext context,
        MessageEnvelope<T> envelope,
        byte? priority,
        byte? maxPriority)
    {
        if (!string.IsNullOrWhiteSpace(envelope.CorrelationId) &&
            Guid.TryParse(envelope.CorrelationId, out var corrId))
        {
            context.CorrelationId = corrId;
        }

        ApplyPriority(context, priority, maxPriority);
    }

    private static void ApplyPriority(SendContext context, byte? priority, byte? maxPriority)
    {
        if (!priority.HasValue)
            return;

        byte effectivePriority = maxPriority is > 0 && priority.Value > maxPriority.Value
            ? maxPriority.Value
            : priority.Value;

        context.Headers.Set("priority", effectivePriority);

        if (context is RabbitMqSendContext rabbitSendContext)
        {
            rabbitSendContext.SetPriority(effectivePriority);
        }
    }
}