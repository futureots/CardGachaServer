using CardGachaServer.Model;
using Microsoft.EntityFrameworkCore;

namespace CardGachaServer.Database;

public class UserDbContext : DbContext
{
    public DbSet<OwnedCharacter> OwnedCharacters { get; set; }
}