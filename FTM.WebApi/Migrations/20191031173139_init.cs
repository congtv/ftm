using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FTM.WebApi.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FtmTokenResponses",
                columns: table => new
                {
                    AccessToken = table.Column<string>(nullable: true),
                    TokenType = table.Column<string>(nullable: true),
                    ExpiresInSeconds = table.Column<long>(nullable: true),
                    RefreshToken = table.Column<string>(nullable: true),
                    Scope = table.Column<string>(nullable: true),
                    IdToken = table.Column<string>(nullable: true),
                    Issued = table.Column<DateTime>(nullable: false),
                    IssuedUtc = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FtmTokenResponses", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "RoomInfos",
                columns: table => new
                {
                    RoomId = table.Column<string>(nullable: false),
                    RoomName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsUseable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomInfos", x => x.RoomId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FtmTokenResponses");

            migrationBuilder.DropTable(
                name: "RoomInfos");
        }
    }
}
