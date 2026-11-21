using Company.Shared.Contracts;
using Company.Shared.Messaging.Consumers;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Worker.Consumers
{
    public class TestMessageConsumer : BaseCompanyConsumer<object>
    {
        public TestMessageConsumer(ILogger<TestMessageConsumer> logger) : base(logger)
        {
        }

        // تطبيق الدالة التي يطلبها الكلاس الأساسي BaseCompanyConsumer
        protected override async Task ProcessMessageAsync(
            object payload,
            MessageEnvelope<object> envelope,
            ConsumeContext<MessageEnvelope<object>> context)
        {
            Logger.LogInformation("==========================================");
            Logger.LogInformation("تم استلام الرسالة بنجاح!");
            Logger.LogInformation("الخدمة المصدر: {SourceService}", envelope.SourceService);
            Logger.LogInformation("محتوى الـ Payload: {Payload}", payload);
            Logger.LogInformation("==========================================");

            await Task.CompletedTask;
        }
    }
}
