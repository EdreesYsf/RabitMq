using Company.Shared.Messaging;
using LibTest;
using MassTransit;
using Microsoft.OpenApi.Models;
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    builder.Services.AddCompanyMessaging(builder.Configuration, cfg =>
    {
        cfg.AddConsumer<NotificationConsumer>();
    });

    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Central Notifications API",
            Version = "v1",
            Description = "API for managing and accessing the central notifications services.",
            Contact = new OpenApiContact
            {
                Name = "Edrees Dbwon",
                Email = "eng.edreesysf@gmail.com"
            },
            License = new OpenApiLicense
            {
                Name = "Copyright © 2026 Tahdeth"
            }
        });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Central Notifications API v1");
        options.RoutePrefix = "swagger"; // Access UI at /swagger
    });

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch(Exception ex)
{

}