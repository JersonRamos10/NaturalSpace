using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaturalSpaceApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageUpdateAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                table: "Messages",
                newName: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Messages",
                newName: "UpdateAt");
        }
    }
}
