using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ScraperLib.Migrations
{
    public partial class MarkersForUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MarkerModelUpdated",
                table: "MarkerModelUpdated");

            migrationBuilder.RenameTable(
                name: "MarkerModelUpdated",
                newName: "MarkersForUpdate");

            migrationBuilder.AddColumn<Guid>(
                name: "OperationGuid",
                table: "MarkersForUpdate",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarkersForUpdate",
                table: "MarkersForUpdate",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MarkersForUpdate",
                table: "MarkersForUpdate");

            migrationBuilder.DropColumn(
                name: "OperationGuid",
                table: "MarkersForUpdate");

            migrationBuilder.RenameTable(
                name: "MarkersForUpdate",
                newName: "MarkerModelUpdated");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarkerModelUpdated",
                table: "MarkerModelUpdated",
                column: "Id");
        }
    }
}
