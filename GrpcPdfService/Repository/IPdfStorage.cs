namespace GrpcPdfService.Repository
{
    public interface IPdfStorage
    {
        Task SaveAsync(string key, byte[] content ,CancellationToken cancellationToken = default);
        Task<byte[]> GetAsync(string key,CancellationToken cancellationToken = default);
        Task DeleteAsync(string key,CancellationToken cancellationToken = default);

    }
}
