using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCinema.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCatalogue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Films",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Synopsis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DureeMinutes = table.Column<int>(type: "int", nullable: false),
                    AffichereUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateSortie = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Films", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Salles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroSalle = table.Column<int>(type: "int", nullable: false),
                    Capacite = table.Column<int>(type: "int", nullable: false),
                    FournisseurId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Salles_AspNetUsers_FournisseurId",
                        column: x => x.FournisseurId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Seances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilmId = table.Column<int>(type: "int", nullable: false),
                    SalleId = table.Column<int>(type: "int", nullable: false),
                    FournisseurId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateHeureDebut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateHeureFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Prix = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FormatFilm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Langue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seances_AspNetUsers_FournisseurId",
                        column: x => x.FournisseurId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seances_Films_FilmId",
                        column: x => x.FilmId,
                        principalTable: "Films",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seances_Salles_SalleId",
                        column: x => x.SalleId,
                        principalTable: "Salles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sieges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroSiege = table.Column<int>(type: "int", nullable: false),
                    SalleId = table.Column<int>(type: "int", nullable: false),
                    Rangee = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TypeSiege = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sieges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sieges_Salles_SalleId",
                        column: x => x.SalleId,
                        principalTable: "Salles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TarifsSeance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeanceId = table.Column<int>(type: "int", nullable: false),
                    Categorie = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Prix = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarifsSeance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TarifsSeance_Seances_SeanceId",
                        column: x => x.SeanceId,
                        principalTable: "Seances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Salles_FournisseurId",
                table: "Salles",
                column: "FournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_Seances_FilmId",
                table: "Seances",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_Seances_FournisseurId",
                table: "Seances",
                column: "FournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_Seances_SalleId",
                table: "Seances",
                column: "SalleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sieges_SalleId_Rangee_NumeroSiege",
                table: "Sieges",
                columns: new[] { "SalleId", "Rangee", "NumeroSiege" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TarifsSeance_SeanceId_Categorie",
                table: "TarifsSeance",
                columns: new[] { "SeanceId", "Categorie" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sieges");

            migrationBuilder.DropTable(
                name: "TarifsSeance");

            migrationBuilder.DropTable(
                name: "Seances");

            migrationBuilder.DropTable(
                name: "Films");

            migrationBuilder.DropTable(
                name: "Salles");
        }
    }
}
