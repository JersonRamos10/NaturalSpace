using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaturalSpaceApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedAtInWorkspace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "WorkSpaces",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "WorkSpaces",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "WorkSpaces");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "WorkSpaces");
        }
    }
}
