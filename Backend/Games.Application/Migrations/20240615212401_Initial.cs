using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Games.Application.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicTacToe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Board = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsGameStarted = table.Column<bool>(type: "bit", nullable: false),
                    GameWinnedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsGameTied = table.Column<bool>(type: "bit", nullable: false),
                    Turn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WinningTiles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Players = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicTacToe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicTacToe");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
