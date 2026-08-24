using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardGachaServer.Database.Migrations.User
{
    /// <inheritdoc />
    public partial class InitUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OwnedCharacters",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnedCharacters", x => new { x.CharacterId, x.UserId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OwnedCharacters");
        }
    }
}
