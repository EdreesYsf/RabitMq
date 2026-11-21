using Company.Shared.Messaging.Extensions;
using Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);

//  ”ÃÌ· Œœ„«  «·„ﬂ »… Ê«·‹ Consumer
builder.Services.AddCompanyMessaging(builder.Configuration, cfg =>
{
    // «” Œœ«„ «”„ «·ﬂ·«” «·„»«‘— TestMessageConsumer
    cfg.AddConsumer<TestMessageConsumer>();
});

var host = builder.Build();
host.Run();