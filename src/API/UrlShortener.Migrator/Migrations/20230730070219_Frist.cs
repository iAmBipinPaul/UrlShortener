using Microsoft.EntityFrameworkCore.Migrations;
using UAParser;
using UrlShortener.Core;

#nullable disable

namespace UrlShortener.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class Frist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShortUrls",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ShortName = table.Column<string>(type: "text", nullable: false),
                    DestinationUrl = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CreationDateTime = table.Column<long>(type: "bigint", nullable: false),
                    LastUpdateDateTime = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortUrls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShortUrlClicks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ShortUrlId = table.Column<string>(type: "text", nullable: false),
                    CreationDateTime = table.Column<long>(type: "bigint", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    IpInfo = table.Column<IpInfo>(type: "jsonb", nullable: true),
                    ClientInfo = table.Column<ClientInfo>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortUrlClicks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShortUrlClicks_ShortUrls_ShortUrlId",
                        column: x => x.ShortUrlId,
                        principalTable: "ShortUrls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShortUrlClicks_ShortUrlId",
                table: "ShortUrlClicks",
                column: "ShortUrlId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShortUrlClicks");

            migrationBuilder.DropTable(
                name: "ShortUrls");
        }
    }
}
