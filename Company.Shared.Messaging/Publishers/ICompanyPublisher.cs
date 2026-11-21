using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Shared.Messaging.Publishers
{
    public interface ICompanyPublisher
    {
        Task PublishAsync<T>(T message, string correlationId = "", byte? priority = null, CancellationToken cancellationToken = default)
            where T : class;

        Task PublishDynamicAsync(
            object dynamicPayload,
            string eventTypeName,
            string correlationId = "",
            byte? priority = null,
            CancellationToken cancellationToken = default);
        Task SendAsync<T>(
T message,
string exchangeName,
string routingKey,
string correlationId = "",
byte? priority = null,
CancellationToken cancellationToken = default) where T : class;
        Task SendToQueueAsync<T>(
            string queueName,
            T message,
            string correlationId = "",
            byte? priority = null,
            CancellationToken cancellationToken = default)
            where T : class;
    }
}
