using GrpcTestResultService.Entities;
using Microsoft.EntityFrameworkCore;

namespace GrpcTestResultService.Model
{
    public class TestResultDbContext(DbContextOptions options) : DbContext(options)
    {
     
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestResult>().
                Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<TestResult>()
                .HasOne(x => x.TestType)
                .WithMany(y=> y.TestResults)
                .HasForeignKey(x=>x.TestTypeId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<TestResult>()
                .HasOne(x=>x.Patient)
                .WithMany(y=>y.TestResults)
                .HasForeignKey(x=>x.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TestType>().Property(e => e.Id).ValueGeneratedOnAdd();   
        }
           public DbSet<TestType> TestTypes { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<Patient> Patients { get; set; } 
    }
}
