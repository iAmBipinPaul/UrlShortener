using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShortUrls",
                columns: table => new
                {
                    ShortName = table.Column<string>(type: "TEXT", nullable: false),
                    DestinationUrl = table.Column<string>(type: "TEXT", nullable: false),
                    CreationDateTime = table.Column<long>(type: "INTEGER", nullable: false),
                    LastUpdateDateTime = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortUrls", x => x.ShortName);
                });

            migrationBuilder.CreateTable(
                name: "ShortUrlClicks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ShortUrlId = table.Column<string>(type: "TEXT", nullable: false),
                    CreationDateTime = table.Column<long>(type: "INTEGER", nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortUrlClicks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShortUrlClicks_ShortUrls_ShortUrlId",
                        column: x => x.ShortUrlId,
                        principalTable: "ShortUrls",
                        principalColumn: "ShortName",
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
