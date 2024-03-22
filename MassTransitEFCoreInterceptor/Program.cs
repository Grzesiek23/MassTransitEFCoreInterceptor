using System.Reflection;
using MassTransit;
using MassTransitEFCoreInterceptor.DbContext;
using MassTransitEFCoreInterceptor.Interceptors;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var isDevelopment = builder.Environment.IsDevelopment();
if (isDevelopment)
{
    builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
}

builder.Services.AddScoped<PublishDomainEventsInterceptor>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WriteDbContext>((sp, options) =>
    options
        .UseSqlServer(builder.Configuration["Infrastructure:SqlServer:ConnectionString"], sqlOptions => { sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null); })
        .AddInterceptors(sp.GetRequiredService<PublishDomainEventsInterceptor>()));

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddEntityFrameworkOutbox<WriteDbContext>(cfg =>
    {
        cfg.UseSqlServer();
        cfg.UseBusOutbox(o => o.DisableDeliveryService());
        cfg.DisableInboxCleanupService();
    });

    x.UsingAzureServiceBus((_, cfg) =>
    {
    });

    x.RemoveMassTransitHostedService();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async (WriteDbContext dbContext) =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);
}