using GrpcPdfService.Entities;
using GrpcPdfService.Model;
using Microsoft.EntityFrameworkCore;

namespace GrpcPdfService.Repository
{
    public class PdfReportRepository(ReportDbContext db) : IPdfRepository
    {
        

        public async Task AddAsync(Report report, CancellationToken cancellationToken = default)
        {
            await db.Reports.AddAsync(report,cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Report>> GetByClientidAsync(int clientId, CancellationToken cancellationToken)
        {
            return await db.Reports.Where(x=>x.ClientId == clientId).AsNoTracking().ToListAsync();
        }

        public async Task<Report> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await db.Reports.FindAsync([id], cancellationToken) ;
        }

        public async Task UpdateAsync(Report report, CancellationToken cancellationToken = default)
        {
             db.Reports.Update(report);
           await db.SaveChangesAsync(cancellationToken);

        }
    }
}
