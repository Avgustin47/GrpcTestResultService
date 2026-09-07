using GrpcTestResultService.Model;
using Microsoft.EntityFrameworkCore;

namespace GrpcTestResultService.Extensions
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddTestResultDbContext(this IServiceCollection services, string connectionstring)
        {
            services.AddDbContext<TestResultDbContext>(opt =>
            {
                opt.UseNpgsql(connectionstring, npgsqlOptionsAction =>
                {
                    npgsqlOptionsAction.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null);

                });
            });
            return services;

        }
    }
}
