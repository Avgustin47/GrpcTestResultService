using GrpcPdfService.Model;
using GrpcPdfService.Repository;
using GrpcPdfService.RMQConsumer;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;


QuestPDF.Settings.License = LicenseType.Community;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddGrpcReflection();
builder.Services.AddDbContext<ReportDbContext>(opt =>
{
    opt.UseNpgsql("Host=reportdb;Database=reportdb;Username=user;Password=qwerty");
});
builder.Services.AddScoped<IPdfRepository, PdfReportRepository>();
builder.Services.AddScoped<IPdfStorage, LocalFileStorage>();
builder.Services.AddMassTransit(busConfigurate =>
{
    busConfigurate.SetKebabCaseEndpointNameFormatter();

    busConfigurate.AddConsumer<PdfConsumer>();
    busConfigurate.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host("rabbitmq", 5672, "/", x =>
        {
            x.Username("admin");
            x.Password("admin123");

        });
        configurator.ConfigureEndpoints(context);

    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReportDbContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
