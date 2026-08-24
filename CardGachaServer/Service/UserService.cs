using CardGachaServer.Database;
using CardGachaServer.Model;
using Microsoft.EntityFrameworkCore;

namespace CardGachaServer.Service;

public interface IUserService
{
    public Task<bool> AddUserData(string userId,Character character);
}
public class UserService : IUserService
{
    private readonly UserDbContext _userDbContext;

    public UserService(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }

    public async Task<bool> AddUserData(string userId, Character character)
    {
        var isExist = await _userDbContext.OwnedCharacters
            .Where(o => o.UserId == userId)
            .AnyAsync(o => o.CharacterId == character.Id);
        if (isExist)
        {
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
        // TODO : 계정 데이터를 확인하고 해당 캐릭터가 존재하면 돌파 재료나 예외 처리를 진행하고 false를 반환. 없을 경우 캐릭터를 추가하고 true를 반환
        return true;
    }
}