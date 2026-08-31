using GrpcAutthorizationService.Model;
using GrpcAutthorizationService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UserDbContext>(o => o.UseNpgsql(
    "Host=usersdb;Database=myuserdb;Username=user;Password=qwerty", npgsqlOptionsAction =>
    {
        npgsqlOptionsAction.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null
            );
    })
    );
// Add services to the container.

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
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

builder.Services.AddAuthorization();
builder.Services.AddMassTransit(busConfigurate =>
{
    busConfigurate.SetKebabCaseEndpointNameFormatter();

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
    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    dbContext.Database.Migrate();
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapGrpcService<AuthJwtService>();
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}



app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
