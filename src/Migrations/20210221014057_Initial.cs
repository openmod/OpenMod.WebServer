using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenMod.WebServer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpenMod_WebServer_AuthTokens",
                columns: table => new
                {
                    Token = table.Column<string>(nullable: false),
                    OwnerType = table.Column<string>(nullable: false),
                    OwnerId = table.Column<string>(nullable: false),
                    ExpirationTime = table.Column<DateTime>(nullable: true),
                    RevokeTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenMod_WebServer_AuthTokens", x => x.Token);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenMod_WebServer_AuthTokens");
        }
    }
}
