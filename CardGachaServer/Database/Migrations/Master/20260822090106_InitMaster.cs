using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardGachaServer.Database.Migrations.Master
{
    /// <inheritdoc />
    public partial class InitMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pools",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ValidateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rarities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rarities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegularCharacters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    RarityId = table.Column<string>(type: "text", nullable: false),
                    IsRegular = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegularCharacters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegularCharacters_Rarities_RarityId",
                        column: x => x.RarityId,
                        principalTable: "Rarities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterPoolRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    PoolId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterPoolRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterPoolRelations_Pools_PoolId",
                        column: x => x.PoolId,
                        principalTable: "Pools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterPoolRelations_RegularCharacters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "RegularCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterPoolRelations_CharacterId",
                table: "CharacterPoolRelations",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterPoolRelations_PoolId_CharacterId",
                table: "CharacterPoolRelations",
                columns: new[] { "PoolId", "CharacterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegularCharacters_Name",
                table: "RegularCharacters",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegularCharacters_RarityId",
                table: "RegularCharacters",
                column: "RarityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterPoolRelations");

            migrationBuilder.DropTable(
                name: "Pools");

            migrationBuilder.DropTable(
                name: "RegularCharacters");

            migrationBuilder.DropTable(
                name: "Rarities");
        }
    }
}
