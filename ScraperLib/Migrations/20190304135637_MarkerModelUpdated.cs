using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace ScraperLib.Migrations
{
    public partial class MarkerModelUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Markers");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Markers");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "Markers",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Markers");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Markers",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Markers",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
