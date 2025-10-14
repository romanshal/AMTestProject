using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NasaSyncService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_meteorites_RecclassId",
                table: "meteorites",
                newName: "IX_Meteorites_RecclassId");

            migrationBuilder.CreateIndex(
                name: "IX_Meteorites_Name",
                table: "meteorites",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Meteorites_YearUtc",
                table: "meteorites",
                column: "YearUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Meteorites_YearUtc_MassGram",
                table: "meteorites",
                columns: new[] { "YearUtc", "MassGram" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Meteorites_Name",
                table: "meteorites");

            migrationBuilder.DropIndex(
                name: "IX_Meteorites_YearUtc",
                table: "meteorites");

            migrationBuilder.DropIndex(
                name: "IX_Meteorites_YearUtc_MassGram",
                table: "meteorites");

            migrationBuilder.RenameIndex(
                name: "IX_Meteorites_RecclassId",
                table: "meteorites",
                newName: "IX_meteorites_RecclassId");
        }
    }
}
