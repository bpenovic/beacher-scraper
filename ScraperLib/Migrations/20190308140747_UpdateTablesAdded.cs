using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace ScraperLib.Migrations
{
    public partial class UpdateTablesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DetailsForUpdate",
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
                    MarkerId = table.Column<int>(nullable: false),
                    OperationGuid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailsForUpdate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarkerModelUpdated",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Location = table.Column<Point>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    City = table.Column<string>(nullable: true),
                    DataId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarkerModelUpdated", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailsForUpdate");

            migrationBuilder.DropTable(
                name: "MarkerModelUpdated");
        }
    }
}
