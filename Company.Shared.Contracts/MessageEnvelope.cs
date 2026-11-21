using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Shared.Contracts
{
    public record MessageEnvelope<T>
    {
        public string MessageId { get; init; } = Guid.NewGuid().ToString("N");
        public string CorrelationId { get; init; } = string.Empty;
        public string SourceService { get; init; } = string.Empty;
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string EventType { get; init; } = typeof(T).Name;

        // الحمولة الديناميكية - تقبل أي كائن أو Dictionary أو JSON
        public T Payload { get; init; } = default!;

        public static MessageEnvelope<T> Create(T payload, string sourceService, string correlationId = "")
        {
            return new MessageEnvelope<T>
            {
                Payload = payload,
                SourceService = sourceService,
                CorrelationId = string.IsNullOrEmpty(correlationId) ? Guid.NewGuid().ToString("N") : correlationId
            };
        }
    }
}
