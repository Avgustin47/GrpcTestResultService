
using GrpcTestResultService.Model;
using GrpcTestResultService.RmqServices;
using GrpcTestResultService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpcReflection();

builder.Services.AddGrpc().AddJsonTranscoding();

builder.Services.AddDbContext<TestResultDbContext>(o => o.UseNpgsql("Host=db;Database=mydb;Username=user;Password=qwerty",
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
        configurator.ConfigureEndpoints(context);

    });
});
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TestResultDbContext>();
    dbContext.Database.Migrate();
}
app.UseAuthentication(); 
app.UseAuthorization();
app.MapGrpcReflectionService();

app.MapGrpcService<TestResultService>();
app.MapGrpcService<TestTypeService>();
app.MapGet("/", () => "");

app.Run();
