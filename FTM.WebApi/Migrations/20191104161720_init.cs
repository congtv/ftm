using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FTM.WebApi.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FtmCalendarInfo",
                columns: table => new
                {
                    CalendarId = table.Column<string>(nullable: false),
                    CalendarName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsUseable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FtmCalendarInfo", x => x.CalendarId);
                });

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FtmCalendarInfo");

            migrationBuilder.DropTable(
                name: "FtmTokenResponses");
        }
    }
}
