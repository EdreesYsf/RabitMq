using Company.Shared.Messaging.Configuration;
using Company.Shared.Messaging.Consumers;
using Company.Shared.Messaging.Publishers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Mail;
using System.Reflection;
 
public static class MessagingExtensions
{
    public static IServiceCollection AddCompanyMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureConsumers = null,
        Action<MessagingOptions>? configureOptions = null)
    {
        services.AddScoped<ICompanyPublisher, CompanyPublisher>();

        var options = new MessagingOptions();
        configuration.GetSection(MessagingOptions.SectionName).Bind(options);
        configureOptions?.Invoke(options);

        services.AddSingleton(options);

        services.AddMassTransit(x =>
        {
            x.SetEndpointNameFormatter(new CustomQueueEndpointNameFormatter());

            if (options.MaxPriority is > 0)
            {
                x.AddConfigureEndpointsCallback((context, name, endpointConfigurator) =>
                {
                    if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbitEndpoint)
                    {
                        rabbitEndpoint.SetQueueArgument("x-max-priority", (int)options.MaxPriority.Value);
                    }
                });
            }

            configureConsumers?.Invoke(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                var host = configuration["RabbitMQ:Host"] ?? "localhost";
                var virtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/";
                var username = configuration["RabbitMQ:Username"] ?? "guest";
                var password = configuration["RabbitMQ:Password"] ?? "guest";

                cfg.Host(host, virtualHost, h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
//public static class MessagingExtensions
//{
//    public static IServiceCollection AddCompanyMessaging(
//        this IServiceCollection services,
//        IConfiguration configuration,
//        Action<IBusRegistrationConfigurator>? configureConsumers = null,
//        Action<MessagingOptions>? configureOptions = null)
//    {
//        // 1. تسجيل واجهة الناشر القياسية للمكتبة
//        services.AddScoped<ICompanyPublisher, CompanyPublisher>();

//        // 2. قراءة الإعدادات من appsettings.json وتطبيق أي تخصيص برمجي
//        var options = new MessagingOptions();
//        configuration.GetSection(MessagingOptions.SectionName).Bind(options);
//        configureOptions?.Invoke(options);

//        services.AddSingleton(options);

//        // 3. إعداد MassTransit مع RabbitMQ
//        services.AddMassTransit(x =>
//        {
//            // تسجيل الـ EndpointNameFormatter المخصص لتطبيق [QueueName]
//            x.SetEndpointNameFormatter(new CustomQueueEndpointNameFormatter());

//            // تسجيل الـ Consumers المارة من مشروع المطور
//            configureConsumers?.Invoke(x);

//            x.UsingRabbitMq((context, cfg) =>
//            {
//                var host = configuration["RabbitMQ:Host"] ?? "localhost";
//                var virtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/";
//                var username = configuration["RabbitMQ:Username"] ?? "guest";
//                var password = configuration["RabbitMQ:Password"] ?? "guest";

//                cfg.Host(host, virtualHost, h =>
//                {
//                    h.Username(username);
//                    h.Password(password);
//                });

//                // تهيئة الـ Endpoints وتفعيل x-max-priority إذا كانت مضبوطة
//                cfg.ConfigureEndpoints(context, endpointConfigurator =>
//                {
//                    if (options.MaxPriority is > 0 &&
//                        endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbitEndpoint)
//                    {
//                        rabbitEndpoint.SetQueueArgument("x-max-priority", (int)options.MaxPriority.Value);
//                    }
//                });
//            });
//        });

//        return services;
//    }
//}
//public static class MessagingExtensions
//{
//    public static IServiceCollection AddCompanyMessaging(
//        this IServiceCollection services,
//        IConfiguration configuration,
//        Action<IBusRegistrationConfigurator>? configureConsumers = null,
//        int? maxPriority = 10)
//    {
//        // 1. تسجيل واجهة الناشر القياسية للمكتبة
//        services.AddScoped<ICompanyPublisher, CompanyPublisher>();
//        var configuredMaxPriority = configuration.GetValue<int?>("RabbitMQ:MaxPriority") ?? maxPriority;
//        // 2. إعداد MassTransit مع RabbitMQ
//        services.AddMassTransit(x =>
//        {
//            // تسجيل الـ Consumers المارة من مشروع المطور
//            configureConsumers?.Invoke(x);

//            x.UsingRabbitMq((context, cfg) =>
//            {
//                // جلب إعدادات الاتصال من appsettings.json مع قيم افتراضية للأمان
//                var host = configuration["RabbitMQ:Host"] ?? "localhost";
//                var virtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/";
//                var username = configuration["RabbitMQ:Username"] ?? "guest";
//                var password = configuration["RabbitMQ:Password"] ?? "guest";

//                cfg.Host(host, virtualHost, h =>
//                {
//                    h.Username(username);
//                    h.Password(password);
//                });

//                // تطبيق التسمية المخصصة للطوابير تلقائياً عبر CustomQueueEndpointNameFormatter
//                //cfg.ConfigureEndpoints(context, new CustomQueueEndpointNameFormatter());
//                // تطبيق التسمية المخصصة للطوابير وتفعيل x-max-priority
//                cfg.ConfigureEndpoints(context, endpointConfigurator =>
//                {
//                    if (configuredMaxPriority.HasValue &&
//                        endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbitEndpoint)
//                    {
//                        rabbitEndpoint.SetQueueArgument("x-max-priority", configuredMaxPriority.Value);
//                    }
//                });
//            });
//        });

//        return services;
//    }
//}

/// <summary>
/// قارئ أسماء الطوابير المخصصة عبر الـ QueueNameAttribute
/// </summary>
public class CustomQueueEndpointNameFormatter : DefaultEndpointNameFormatter
{
    public override string Consumer<T>()
    {
        var queueAttr = typeof(T).GetCustomAttribute<QueueNameAttribute>();

        if (queueAttr != null && !string.IsNullOrWhiteSpace(queueAttr.Name))
        {
            return queueAttr.Name;
        }

        // في حال عدم وجود الـ Attribute يتم الاعتماد على الاسم الافتراضي من MassTransit
        return base.Consumer<T>();
    }
}