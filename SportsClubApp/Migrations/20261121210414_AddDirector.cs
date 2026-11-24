using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsClubApp.Migrations
{
    /// <inheritdoc />
    public partial class AddDirector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Riders",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    League = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Riders_TeamId",
                table: "Riders",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Riders_Teams_TeamId",
                table: "Riders",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Riders_Teams_TeamId",
                table: "Riders");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Riders_TeamId",
                table: "Riders");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Riders");
        }
    }
}
