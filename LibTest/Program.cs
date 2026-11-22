 
 using Company.Shared.Messaging.Extensions;
using LibTest;
using MassTransit; 
using Microsoft.Extensions.Options; 
var builder = WebApplication.CreateBuilder(args);
try
{
    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();
    builder.Services.AddCompanyMessaging(builder.Configuration, cfg =>
    {
        cfg.AddConsumer<NotificationConsumer>();
    });
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Central Notufications API",
            Version = "v1",
            Description = "API for managing and accessing the central notufications services.",

            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "Edrees Dbwon",
                Email = "eng.edreesysf@gmail.com",
                //Url = new Uri("")
            },

            License = new Microsoft.OpenApi.Models.OpenApiLicense
            {
                Name = "Copyright © 2026 Tahdeth",
                //Url = new Uri("")
            }
        });
    });
    var app = builder.Build();
    app.UseSwagger();

    // 2. ÊÝÚíá æÇÌåÉ ãÓÊÎÏã Swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Central Notifications API v1");
        // options.SwaggerEndpoint("/swagger/v2/swagger.json", "Central Notifications API v2");
    });
    // Configure the HTTP request pipeline.
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