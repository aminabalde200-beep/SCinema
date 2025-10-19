using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCinema.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameCreatedAtToCreateAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "AspNetUsers",
                newName: "CreateAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreateAt",
                table: "AspNetUsers",
                newName: "CreatedAt");
        }
    }
}
