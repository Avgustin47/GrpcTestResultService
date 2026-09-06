using GrpcTestResultService.Caching;
using GrpcTestResultService.Model;
using GrpcTestResultService.RmqServices;
using GrpcTestResultService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Services.AddHealthChecks();
var redisConfig = builder.Configuration.GetSection("Redis");
if (redisConfig.GetValue<bool>("Enabled", true))
{
    builder.Services.AddStackExchangeRedisCache(opt =>
    {
        opt.Configuration = redisConfig.GetValue<string>("ConnectionString");
        opt.InstanceName = redisConfig.GetValue<string>("InstanceName", "TestResultService");
    });

    var connectionStringredis = redisConfig.GetValue<string>("ConnectionString");

    var redisConnetion = ConnectionMultiplexer.Connect(connectionStringredis!);

    builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnetion);

    builder.Services.AddScoped<ICacheService, RedisCacheService>();
}
else
{
    builder.Services.AddScoped<ICacheService, NullCacheSevice>();
}


builder.Services.AddGrpcReflection();

builder.Services.AddGrpc(opt =>
{
    opt.EnableDetailedErrors = true;

}).AddJsonTranscoding();


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Application", "GrpcTestResultService")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/logs-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();

builder.Host.UseSerilog();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TestResultDbContext>(o => o.UseNpgsql(connectionString,
     npgsqlOptions =>
     {
         npgsqlOptions.EnableRetryOnFailure(
             maxRetryCount: 5,
             maxRetryDelay: TimeSpan.FromSeconds(10),
             errorCodesToAdd: null);
     }));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddMassTransit(busConfigurate =>
{
    busConfigurate.SetKebabCaseEndpointNameFormatter();

    busConfigurate.AddConsumer<MqBackServ>();
    busConfigurate.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host("rabbitmq", 5672, "/", x =>
        {
            x.Username("admin");
            x.Password("admin123");

        });
        configurator.UseMessageRetry(retryConfig =>
        {
            retryConfig.Interval(3, TimeSpan.FromSeconds(5));
        });
        configurator.ConfigureEndpoints(context);

    });
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() =>
{
    Log.Information("Application started successfully");
});
app.Lifetime.ApplicationStopping.Register(() =>
{
    Log.Information("Application is stopping...");
  
});
app.Lifetime.ApplicationStopped.Register(() =>
{
    Log.Information("Application stopped");
});


app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcReflectionService();
app.MapHealthChecks("/health");
app.MapGrpcService<TestResultService>();
app.MapGrpcService<TestTypeService>();
app.MapGet("/", () => "");

app.Run();
Log.CloseAndFlush();