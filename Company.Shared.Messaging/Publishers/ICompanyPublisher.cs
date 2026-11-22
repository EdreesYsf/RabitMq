using Company.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Company.Shared.Messaging.Publishers
{
    public interface ICompanyPublisher
    {
        Task PublishAsync<T>(
            MessageEnvelope<T> envelope,
            byte? priority = null,
            CancellationToken cancellationToken = default)
            where T : class;

        Task PublishDynamicAsync(
            MessageEnvelope<JsonElement> envelope,
            byte? priority = null,
            CancellationToken cancellationToken = default);

        Task SendAsync<T>(
            MessageEnvelope<T> envelope,
            string exchangeName,
            string routingKey,
            byte? priority = null,
            CancellationToken cancellationToken = default)
            where T : class;

        Task SendToQueueAsync<T>(
            string queueName,
            MessageEnvelope<T> envelope,
            byte? priority = null,
            CancellationToken cancellationToken = default)
            where T : class;
    }
}
