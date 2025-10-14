using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCinema.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFilmAfficheUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateHeureFin",
                table: "Seances",
                newName: "HeureFin");

            migrationBuilder.RenameColumn(
                name: "DateHeureDebut",
                table: "Seances",
                newName: "HeureDebut");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateSeance",
                table: "Seances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AfficheUrl",
                table: "Films",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateSeance",
                table: "Seances");

            migrationBuilder.DropColumn(
                name: "AfficheUrl",
                table: "Films");

            migrationBuilder.RenameColumn(
                name: "HeureFin",
                table: "Seances",
                newName: "DateHeureFin");

            migrationBuilder.RenameColumn(
                name: "HeureDebut",
                table: "Seances",
                newName: "DateHeureDebut");
        }
    }
}
