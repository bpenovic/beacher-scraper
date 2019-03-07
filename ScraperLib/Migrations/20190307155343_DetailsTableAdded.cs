using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ScraperLib.Migrations
{
    public partial class DetailsTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Details",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(maxLength: 50, nullable: true),
                    SurfaceType = table.Column<string>(maxLength: 50, nullable: true),
                    Vegetation = table.Column<string>(maxLength: 100, nullable: true),
                    Shape = table.Column<string>(maxLength: 100, nullable: true),
                    AverageTemperature = table.Column<double>(nullable: false),
                    MaxSalinity = table.Column<double>(nullable: false),
                    MinSalinity = table.Column<double>(nullable: false),
                    Wind = table.Column<string>(maxLength: 50, nullable: true),
                    Length = table.Column<double>(nullable: false),
                    Width = table.Column<double>(nullable: false),
                    MarkerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Details_Markers_MarkerId",
                        column: x => x.MarkerId,
                        principalTable: "Markers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Details_MarkerId",
                table: "Details",
                column: "MarkerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Details");
        }
    }
}
