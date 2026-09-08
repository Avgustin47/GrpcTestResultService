using GrpcPdfService.Entities;

namespace GrpcPdfService.Repository
{
    public interface IPdfRepository
    {
        Task AddAsync(Report report, CancellationToken cancellationToken = default);
        Task<List<Report>> GetByClientidAsync(int clientId, CancellationToken cancellationToken);
        Task<Report> GetByIdAsync(int id,CancellationToken cancellationToken = default);
        Task UpdateAsync(Report report, CancellationToken cancellationToken = default);
    }
}
