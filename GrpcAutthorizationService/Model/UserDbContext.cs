using GrpcAutthorizationService.Entitites;
using Microsoft.EntityFrameworkCore;

namespace GrpcAutthorizationService.Model
{
    public class UserDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

    }
}
