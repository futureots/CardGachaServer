using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using CardGachaServer.Database;
using CardGachaServer.Model;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace CardGachaServer.Service;

public interface IAuthService
{
    public Task<LoginResponse?> LoginGoogle(string idToken);
    public Task<RefreshResponse?> RefreshAsync(string refreshToken);
}

public class AuthService : IAuthService
{
    private const int TokenValidMinute = 60;
    private readonly AuthDbContext _dbContext;
    private readonly IConfiguration _config;
    private readonly IDatabase _redis;
    public AuthService(AuthDbContext dbContext, IConfiguration config, IConnectionMultiplexer redis)
    {
        _dbContext = dbContext;
        _config = config;
        _redis = redis.GetDatabase();
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
        
        var accessToken = GenerateAccessToken(data.Id);
        var refreshToken = await GenerateRefreshTokenAsync(data.Id);
        
        return new LoginResponse(accessToken.token, refreshToken, accessToken.expiredAt, name);
    }

    public async Task<RefreshResponse?> RefreshAsync(string refreshToken)
    {
        var userId = await ValidateRefreshTokenAsync(refreshToken);
        if (userId == null)
            return null;
        
        // 기존 토큰 제거
        await RevokeRefreshTokenAsync(refreshToken);
        
        // 새로운 엑세스 토큰과 리프레시 토큰 발급(RTR 방식 사용)
        var newAccessToken = GenerateAccessToken(userId);
        var newRefreshToken = await GenerateRefreshTokenAsync(userId);
        
        return new RefreshResponse(newAccessToken.token, newRefreshToken, newAccessToken.expiredAt);
    }

    (string token,DateTime expiredAt) GenerateAccessToken(string userId)
    {
        var rsa = RSA.Create();
        var securityKey = new RsaSecurityKey(rsa);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
        
        var claims = new []
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
        };
        var expiredAt = DateTime.UtcNow.AddMinutes(TokenValidMinute);
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiredAt,
            signingCredentials: credentials
        );
        
        return new (new JwtSecurityTokenHandler().WriteToken(token), expiredAt);
    }
    
    // TODO : 나중에 
    async Task<string> GenerateRefreshTokenAsync(string userId)
    {
        var rand = RandomNumberGenerator.GetBytes(64);
        var refreshToken = Convert.ToBase64String(rand)
            .Replace("+", "-").Replace("/", "_").Replace("=", "");
        
        var key = $"refresh:{refreshToken}";
        await _redis.StringSetAsync(key, userId,TimeSpan.FromDays(14));
        
        return refreshToken;
    }
    
    async Task<string?> ValidateRefreshTokenAsync(string refreshToken)
    {
        var key = $"refresh:{refreshToken}";
        var storedUserId = await _redis.StringGetAsync(key);
        return storedUserId.HasValue ? storedUserId.ToString() : null;
    }
    async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var key = $"refresh:{refreshToken}";
        await _redis.KeyDeleteAsync(key);
    }
    
}

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiredAt,
    string Name
    );

public record RefreshResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiredAt);