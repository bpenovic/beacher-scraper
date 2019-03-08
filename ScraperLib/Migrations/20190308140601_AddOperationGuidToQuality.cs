using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ScraperLib.Migrations
{
    public partial class AddOperationGuidToQuality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OperationGuid",
                table: "QualitiesForUpdate",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperationGuid",
                table: "QualitiesForUpdate");
        }
    }
}
