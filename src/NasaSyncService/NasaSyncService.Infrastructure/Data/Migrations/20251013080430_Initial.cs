using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NasaSyncService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "recclasses",
                columns: table => new
                {
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecclassName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recclasses", x => x.ClassId);
                });

            migrationBuilder.CreateTable(
                name: "snapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SnapshotHash = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    SourceUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    FetchedCount = table.Column<int>(type: "integer", nullable: false),
                    InsertedCount = table.Column<int>(type: "integer", nullable: false),
                    UpdatedCount = table.Column<int>(type: "integer", nullable: false),
                    SoftDeletedCount = table.Column<int>(type: "integer", nullable: false),
                    SkippedSameHash = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    Error = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_snapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "meteorites",
                columns: table => new
                {
                    MetioriteId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Nametype = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RecclassId = table.Column<Guid>(type: "uuid", nullable: false),
                    MassGram = table.Column<decimal>(type: "numeric", nullable: true),
                    Fall = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    YearUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Reclat = table.Column<double>(type: "double precision", nullable: true),
                    Reclong = table.Column<double>(type: "double precision", nullable: true),
                    Extra = table.Column<string>(type: "text", nullable: true),
                    RecordHash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meteorites", x => x.MetioriteId);
                    table.ForeignKey(
                        name: "FK_meteorites_recclasses_RecclassId",
                        column: x => x.RecclassId,
                        principalTable: "recclasses",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "geolocations",
                columns: table => new
                {
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeteoriteId = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_geolocations", x => x.LocationId);
                    table.ForeignKey(
                        name: "FK_geolocations_meteorites_MeteoriteId",
                        column: x => x.MeteoriteId,
                        principalTable: "meteorites",
                        principalColumn: "MetioriteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_geolocations_MeteoriteId",
                table: "geolocations",
                column: "MeteoriteId");

            migrationBuilder.CreateIndex(
                name: "IX_meteorites_RecclassId",
                table: "meteorites",
                column: "RecclassId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "geolocations");

            migrationBuilder.DropTable(
                name: "snapshots");

            migrationBuilder.DropTable(
                name: "meteorites");

            migrationBuilder.DropTable(
                name: "recclasses");
        }
    }
}
