﻿using Microsoft.EntityFrameworkCore.Migrations;

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
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salt = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "TicTacToe");
        }
    }
}
