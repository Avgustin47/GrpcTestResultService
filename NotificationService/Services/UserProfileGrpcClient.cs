using Grpc.Core;
using GrpcAutthorizationService.Protos;

namespace NotificationService.Services
{
    public class UserProfileGrpcClient(UserProfileSerice.UserProfileSericeClient client) : IUserProfileClient
    {
        

        public async Task<string> GetEmailByClientId(Guid clientId, CancellationToken cancellationToken = default)
        {
            var request = new GetUserProfileRequest { ClientId = clientId.ToString() };
            try
            {
                var response = await client.GetUserProfileAsync(request,cancellationToken : cancellationToken);
                return response.Email;
            }
            catch(RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                throw new Exception($"User with clientId{clientId} not found", ex);
            }
        }
    }
}
