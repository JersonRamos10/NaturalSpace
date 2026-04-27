using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaturalSpaceApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameFilePatchToFilePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilePatch",
                table: "Files",
                newName: "FilePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Files",
                newName: "FilePatch");
        }
    }
}
