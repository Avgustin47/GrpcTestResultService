using Grpc.Core;
using GrpcAutthorizationService.Entitites;
using GrpcAutthorizationService.Model;
using GrpcAutthorizationService.Protos;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UserMqDto;


namespace GrpcAutthorizationService.Services
{
    public class AuthJwtService(UserDbContext co, IPublishEndpoint endp) : AuthService.AuthServiceBase
    {

        private readonly UserDbContext db = co;
        private readonly IPublishEndpoint _publishEndpoint = endp;
        public override async Task<RegisterRespone> Register(RegisterRequest request, ServerCallContext context)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FullName = request.Fullname,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();


            await _publishEndpoint.Publish(new UserDto { Id = user.Id, Name = user.FullName });


            var response = new RegisterRespone { UserId = user.Id.ToString(), Succes = true, Message = "регистрация прошла успешно" };
            return await Task.FromResult(response);
        }

        public override async Task<LoginResponce> Login(LoginRequest request, ServerCallContext context)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return new LoginResponce { Succes = false, Message = "неправильный логин или пароль" };
            
            string token = GenerateJwt.Generate(user, request.Role);
            string refreshtoken = GenerateJwt.GenerateRefreshToken();
            user.RefreshToken = refreshtoken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await db.SaveChangesAsync();
            return new LoginResponce
            {
                Succes = true,
                AccessToken = token,
                RefreshToken = refreshtoken,
                UserId = user.Id.ToString(),
                Message = "Вход успешен"
            };
        }
        [Authorize(Roles = "admin")]
        public override async Task<RefreshTokenResponce> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.RefreshToken == request.RefreshToken);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return new RefreshTokenResponce
                {
                    Succes = false,
                    Message = "Invalid or expired token"
                };

            }
            string newtoken = GenerateJwt.Generate(user, request.Role);
            string newrefreshtoken = GenerateJwt.GenerateRefreshToken();

            user.RefreshToken = newrefreshtoken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await db.SaveChangesAsync();

            return new RefreshTokenResponce
            {
                Succes = true,
                AccessToken = newtoken,
                RefreshToken = newrefreshtoken,
                Message = "Token refreshed succesfully"
            };
        }

    }
}
