using CardGachaServer.Database;
using CardGachaServer.Model;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;

namespace CardGachaServer.Service;

public interface IAuthService
{
    public Task<LoginResponse?> LoginGoogle(string idToken);
    public Task<RefreshResponse?> Refresh(string refreshToken);
}

public class AuthService : IAuthService
{
    private readonly AuthDbContext _dbContext;

    public AuthService(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LoginResponse?> LoginGoogle(string idToken)
    {
        GoogleJsonWebSignature.Payload payload;
        // 인증 확인
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
        }
        catch(Exception e)
        {
            Console.Write(e);
            return null;
        }

        var provider = "Google";
        var providerId = payload.Subject;
        var name = payload.Name ?? payload.Email;

        // 계정 존재 여부 확인
        var data = await _dbContext.Users
            .Where(u => u.ProviderId == providerId)
            .FirstOrDefaultAsync();
        
        // 계정이 존재하지 않는 경우 추가
        if (data == null)
        {
            data = new User
            {
                Name = name,
                ProviderId = providerId,
                Provider = provider,
            };
            _dbContext.Users.Add(data);
            await _dbContext.SaveChangesAsync();
        }

        // TODO : accessToken 및 refreshToken 구현
        var accessToken = CreateAccessToken(data);
        var refreshToken = CreateAndStoreRefreshToken(data);
        // TODO : refreshToken db에 저장, 나중에 redis에 저장
        return new LoginResponse(accessToken, refreshToken, name);
    }

    public async Task<RefreshResponse?> Refresh(string refreshToken)
    {
        return null;
    }

    string CreateAccessToken(User user)
    {
        return string.Empty;
    }

    string CreateAndStoreRefreshToken(User user)
    {
        return string.Empty;
    }
}

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    string Name
    );

public record RefreshResponse(
    string AccessToken,
    string RefreshToken);