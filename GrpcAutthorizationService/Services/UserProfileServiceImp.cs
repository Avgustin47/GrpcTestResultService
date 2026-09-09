using Grpc.Core;
using GrpcAutthorizationService.Model;
using GrpcAutthorizationService.Protos;
using Microsoft.EntityFrameworkCore;

namespace GrpcAutthorizationService.Services
{
    public class UserProfileServiceImp(UserDbContext db, ILogger<UserProfileServiceImp> logger) : UserProfileSerice.UserProfileSericeBase
    {
        public override async Task<UserProfileResponce> GetUserProfile(GetUserProfileRequest request, ServerCallContext context)
        {
            logger.LogInformation("GetUserProfile вызван для client_id :{clientID}", request.ClientId);
            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.ClientId), cancellationToken: context.CancellationToken);
            if (user == null)
            {
                logger.LogWarning("Пользователь с client_id {ClientId} не найден", request.ClientId);
                throw new RpcException(new Status(StatusCode.NotFound, "User не найден"));
            }
            return new UserProfileResponce
            {
                ClientId = user.Id.ToString(),
                Email = user.Email,

            };
        }
    }
}
