
using GrpcPdfService.RMQConsumer;
using GrpcPdfService.Services;
using MassTransit;
using QuestPDF.Infrastructure;


QuestPDF.Settings.License = LicenseType.Community;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddGrpcReflection();
builder.Services.AddGrpcClient<PdfService>();
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

app.MapGrpcReflectionService();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
