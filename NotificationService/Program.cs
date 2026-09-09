using Grpc.Net.Client;
using GrpcAutthorizationService.Protos;
using MassTransit;
using NotificationService.MqConsumer;
using NotificationService.Services;
using StackExchange.Redis;
using UserMqDto;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddGrpc();
builder.Services.AddMassTransit(busConfigurate =>
{
    busConfigurate.SetKebabCaseEndpointNameFormatter();

    busConfigurate.AddConsumer<ReportReadyConsumer>();
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
builder.Services.AddSingleton<GrpcChannel>(sp =>
{
    var adress = "http://grpcautharizationtservice:8080";
    return GrpcChannel.ForAddress(adress, new GrpcChannelOptions { HttpHandler = new HttpClientHandler() });

});
builder.Services.AddScoped<UserProfileSerice.UserProfileSericeClient>(sp =>
{
    var channel = sp.GetRequiredService<GrpcChannel>();
    return new UserProfileSerice.UserProfileSericeClient(channel);

});
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var redisConnectionString = configuration.GetConnectionString("Redis") ?? "redis:6379,password=qwerty,defaultDatabase=0";
    return ConnectionMultiplexer.Connect(redisConnectionString);
});
builder.Services.AddScoped<ITokenStorage,RedisTokenStorage>();
builder.Services.AddScoped<IUserProfileClient, UserProfileGrpcClient>();
builder.Services.AddScoped<IEmailService,SmtpEmailService>();
var app = builder.Build();


// Configure the HTTP request pipeline.

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
