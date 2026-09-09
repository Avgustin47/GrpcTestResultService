
using GrpcTestResultService.Extensions;
using GrpcTestResultService.RmqServices;
using GrpcTestResultService.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Services.AddHealthChecks();
builder.Services.AddRedisCashing(builder.Configuration);
builder.Services.AddGrpcReflection();
builder.Services.AddGrpc(opt =>
{
    opt.EnableDetailedErrors = true;

}).AddJsonTranscoding();
builder.AddSerilogLogging();
var connectionString = "Host=db;Database=mydb;Username=user;Password=qwerty";
builder.Services.AddTestResultDbContext(connectionString);
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddMassTransitWithRabbitMq(bus =>
{
    bus.AddConsumer<MqBackServ>();
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() => Log.Information("Application started successfully"));
app.Lifetime.ApplicationStopping.Register(() => Log.Information("Application is stopping..."));
app.Lifetime.ApplicationStopped.Register(() => Log.Information("Application stopped"));


app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcReflectionService();
app.MapHealthChecks("/health");

app.MapGrpcService<TestResultService>();
app.MapGrpcService<TestTypeService>();
app.MapGet("/", () => "");

app.Run();
Log.CloseAndFlush();