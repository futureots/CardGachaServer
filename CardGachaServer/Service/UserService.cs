using System.Security.Claims;
using CardGachaServer.Database;
using CardGachaServer.Model;
using Microsoft.EntityFrameworkCore;

namespace CardGachaServer.Service;

public interface IUserService
{
    /// <summary>
    /// 초기 로그인 및 상태확인
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public Task<User?> GetOrCreateUser(ClaimsPrincipal principal);
    
    /// <summary>
    /// 이름 변경 서비스
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userName"></param>
    /// <returns></returns>
    public Task<User?> UpdateUserNameAsync(string userId, string userName);
    
    public Task<bool> AddOwnedCharacter(string userId,Character character);
    public Task<List<CharacterData>> GetUserCharacters(string userId);
}
public class UserService : IUserService
{
    private readonly UserDbContext _userDbContext;

    public UserService(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }

    public async Task<User?> GetOrCreateUser(ClaimsPrincipal principal)
    {
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return null;
        var user = await _userDbContext.Users.FirstOrDefaultAsync(u => u.FirebaseUid == userId);
        if (user == null)
        {
            user = new User()
            {
                FirebaseUid = userId,
                Name = DefaultNameGenerator.Generate(),
                IsBanned = false,
                CreatedAt = DateTime.UtcNow,
            }; 
            _userDbContext.Users.Add(user);
        }
        await _userDbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User?> UpdateUserNameAsync(string userId, string userName)
    {
        var user = await _userDbContext.Users.FirstOrDefaultAsync(u => u.FirebaseUid == userId);
        if (user == null) return null;
        user.Name = userName;
        await _userDbContext.SaveChangesAsync();
        return user;
    }

    public async Task<bool> AddOwnedCharacter(string userId, Character character)
    {
        var existCharacter = await _userDbContext.OwnedCharacters
            .Where(o => o.UserId == userId)
            .FirstOrDefaultAsync(o => o.CharacterId == character.Id);
        if (existCharacter != null)
        {
            // 일단 중복일 경우 해당 캐릭터의 카운트만 상승
            existCharacter.Count = existCharacter.Count + 1;
            await _userDbContext.SaveChangesAsync();
            return false;
        }

        var owned = new OwnedCharacter()
        {
            UserId =  userId,
            CharacterId = character.Id,
            // TODO : 레벨과 같은 초기값은 클래스 내부에서 미리 설정
        };
        _userDbContext.OwnedCharacters.Add(owned);
        await _userDbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<List<CharacterData>> GetUserCharacters(string userId)
    {
        var data = await _userDbContext.OwnedCharacters
            .Where(o => o.UserId == userId)
            .Select(o => new CharacterData(
                o.CharacterId,
                o.Level))
            .ToListAsync();
        return data;
    }
    
}

public record CharacterData(string CharacterId, int Level);