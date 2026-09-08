namespace GrpcPdfService.Repository
{
   
    public class LocalFileStorage : IPdfStorage
    {
        private readonly string basePath;

        public LocalFileStorage(IConfiguration config)
        {
            basePath = config["Sorage:BasePath"] ?? "/app/reports";
            Directory.CreateDirectory(basePath);
        }

        public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
            string fullpath = Path.Combine(basePath, key);
            if (File.Exists(fullpath))
                File.Delete(fullpath);
            return Task.CompletedTask;
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            string fullPath = Path.Combine(basePath, key);
            if(!File.Exists(fullPath))
                throw new FileNotFoundException("Файл не найден",key);
            return await File.ReadAllBytesAsync(fullPath, cancellationToken);
        }

        public async Task SaveAsync(string key, byte[] content, CancellationToken cancellationToken = default)
        {
            string fullPath = Path.Combine(basePath, key);
            string direction = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(direction))
                Directory.CreateDirectory(direction);
            await File.WriteAllBytesAsync(fullPath, content, cancellationToken) ;
        }
    }
}
