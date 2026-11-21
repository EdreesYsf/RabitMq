using Company.Shared.Contracts; 
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Company.Shared.Messaging.Consumers;

public abstract class BaseCompanyConsumer<T> : IConsumer<MessageEnvelope<T>>
    where T : class
{
    protected readonly ILogger Logger;

    protected BaseCompanyConsumer(ILogger logger)
    {
        Logger = logger;
    }

    public async Task Consume(ConsumeContext<MessageEnvelope<T>> context)
    {
        var envelope = context.Message;

        Logger.LogInformation(
            "تم استقبال الرسالة [{EventType}] من الخدمة [{SourceService}] | TraceId: {CorrelationId}",
            envelope.EventType, envelope.SourceService, envelope.CorrelationId);

        await ProcessMessageAsync(envelope.Payload, envelope, context);
    }

    protected abstract Task ProcessMessageAsync(
        T payload,
        MessageEnvelope<T> envelope,
        ConsumeContext<MessageEnvelope<T>> context);
}