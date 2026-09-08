using GrpcPdfService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace GrpcPdfService.Model
{

    public class ReportDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Report> Reports { get; set; }
    }

}
