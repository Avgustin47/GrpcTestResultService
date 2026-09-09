namespace NotificationService.Services
{
    public interface IUserProfileClient
    {
        Task<string> GetEmailByClientId(Guid clientId, CancellationToken cancellationToken = default);
    }
}
