# Company.Shared.Messaging

A standardized .NET 9 messaging library built on **MassTransit** and **RabbitMQ**, providing consistent publishing/sending patterns, custom queue naming via `[QueueName]`, and configurable message priority support (`x-max-priority`).

---

## Features

* **Standardized Publisher (`ICompanyPublisher` / `CompanyPublisher`):** Consistent message envelope wrapping, correlation ID support, routing key support, and optional `byte? priority` on send/publish operations.
* **Configurable Queue Priority (`x-max-priority`):** Control maximum queue priority levels via `appsettings.json` (`Messaging:MaxPriority`) or programmatic options (`MessagingOptions`), with safe fallback defaults and optional disablement (`null` or `<= 0`).
* **Custom Queue Naming (`CustomQueueEndpointNameFormatter`):** Automatically resolves queue names from `[QueueName]` attributes on consumers, falling back to MassTransit's default endpoint naming convention.

---

## Installation & Registration

Register the messaging services in your `Program.cs` and pass your consumers via `configureConsumers`:

```csharp
using Company.Shared.Messaging.Extensions;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCompanyMessaging(
    builder.Configuration,
    configureConsumers: x =>
    {
        x.AddConsumer<NotificationConsumer>();
    });

var app = builder.Build();
app.Run();
```

You can also override or customize `MessagingOptions` in code:

```csharp
builder.Services.AddCompanyMessaging(
    builder.Configuration,
    configureConsumers: x =>
    {
        x.AddConsumer<NotificationConsumer>();
    },
    configureOptions: options =>
    {
        options.MaxPriority = 20; // Set to null to disable x-max-priority
    });
```

---

## Configuration (`appsettings.json`)

Configure RabbitMQ connection details and messaging options in `appsettings.json`:

```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "VirtualHost": "/",
    "Username": "guest",
    "Password": "guest"
  },
  "Messaging": {
    "MaxPriority": 10
  }
}
```

### Configuration Behavior

* **`Messaging:MaxPriority`:**
  * Sets the RabbitMQ `x-max-priority` queue argument when endpoints are configured.
  * Defaults to `10` in `MessagingOptions` if omitted from configuration.
  * Any `byte` value can be passed as `priority` when sending/publishing messages (not restricted to a fixed enum ceiling). RabbitMQ orders messages up to the queue's `x-max-priority` ceiling (values above the queue's max priority are treated at the queue's maximum).
  * Set to `null` (or `0` / negative) if you do not want `x-max-priority` applied to queues.

---

## Custom Queue Naming (`[QueueName]` & `CustomQueueEndpointNameFormatter`)

Decorate a consumer class with `[QueueName]` to give its receive endpoint a custom queue name:

```csharp
using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class QueueNameAttribute : Attribute
{
    public string Name { get; }

    public QueueNameAttribute(string name)
    {
        Name = name;
    }
}
```

Example consumer using `[QueueName]`:

```csharp
using System.Threading.Tasks;
using MassTransit;

[QueueName("notification-orders-queue")]
public sealed class NotificationConsumer : IConsumer<MessageEnvelope<OrderNotification>>
{
    public Task Consume(ConsumeContext<MessageEnvelope<OrderNotification>> context)
    {
        var payload = context.Message.Data;
        var priorityHeader = context.Headers.Get<byte?>("priority");

        // Process notification...
        return Task.CompletedTask;
    }
}
```

`CustomQueueEndpointNameFormatter` is registered via `x.SetEndpointNameFormatter(...)` during `AddMassTransit` setup, so `[QueueName]` is automatically applied during `cfg.ConfigureEndpoints(...)`. If a consumer does not specify `[QueueName]`, MassTransit's default endpoint naming is used.

---

## Sending & Publishing Messages with Priority

Inject `ICompanyPublisher` into your application services or handlers and pass an optional `byte? priority`:

```csharp
using System.Threading;
using System.Threading.Tasks;

public sealed class OrderService
{
    private readonly ICompanyPublisher _publisher;

    public OrderService(ICompanyPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task NotifyOrderAsync(OrderNotification notification, CancellationToken ct = default)
    {
        await _publisher.SendAsync(
            message: notification,
            exchangeName: "notifications.exchange",
            routingKey: "notifications.orders",
            correlationId: notification.OrderId.ToString(),
            priority: 10,
            cancellationToken: ct);
    }
}
```

When `priority` has a value, `CompanyPublisher` sets the `priority` header on the outgoing `SendContext`. When `priority` is `null`, no custom priority header is set.

---

## Important Operational Notes

* **Changing `x-max-priority` on Existing Queues:** RabbitMQ treats queue arguments such as `x-max-priority` as immutable once a queue is declared. If a queue already exists in RabbitMQ with a different `x-max-priority` (or without it), delete the existing queue in RabbitMQ Management and restart the service so it is recreated with the updated argument.
* **Priority Ordering Requires Queue Backlog:** Message priority ordering in RabbitMQ takes effect when messages wait in the queue faster than consumers drain them. If consumers process messages immediately as they arrive, queues stay empty and messages are handled essentially as they arrive.