using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Digin_Kompetanse.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bedrift",
                columns: table => new
                {
                    bedrift_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bedrift_navn = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    bedrift_epost = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    beskrivelse = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("bedrift_pkey", x => x.bedrift_id);
                });

            migrationBuilder.CreateTable(
                name: "kompetanse",
                columns: table => new
                {
                    kompetanse_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    kompetanse_kategori = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("kompetanse_pkey", x => x.kompetanse_id);
                });

            migrationBuilder.CreateTable(
                name: "Fagområde",
                columns: table => new
                {
                    fagområde_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fagområde_navn = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    bedrift_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Fagområde_pkey", x => x.fagområde_id);
                    table.ForeignKey(
                        name: "fk_bedrift",
                        column: x => x.bedrift_id,
                        principalTable: "bedrift",
                        principalColumn: "bedrift_id");
                });

            migrationBuilder.CreateTable(
                name: "under_kompetanse",
                columns: table => new
                {
                    underkompetanse_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    underkompetanse_navn = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    kompetanse_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("under_kompetanse_pkey", x => x.underkompetanse_id);
                    table.ForeignKey(
                        name: "under_kompetanse_kompetanse_id_fkey",
                        column: x => x.kompetanse_id,
                        principalTable: "kompetanse",
                        principalColumn: "kompetanse_id");
                });

            migrationBuilder.CreateTable(
                name: "fagområde_has_kompetanse",
                columns: table => new
                {
                    fagområde_id = table.Column<int>(type: "integer", nullable: false),
                    kompetanse_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("fagområde_has_kompetanse_pkey", x => new { x.fagområde_id, x.kompetanse_id });
                    table.ForeignKey(
                        name: "fagområde_has_kompetanse_fagområde_id_fkey",
                        column: x => x.fagområde_id,
                        principalTable: "Fagområde",
                        principalColumn: "fagområde_id");
                    table.ForeignKey(
                        name: "fagområde_has_kompetanse_kompetanse_id_fkey",
                        column: x => x.kompetanse_id,
                        principalTable: "kompetanse",
                        principalColumn: "kompetanse_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fagområde_bedrift_id",
                table: "Fagområde",
                column: "bedrift_id");

            migrationBuilder.CreateIndex(
                name: "IX_fagområde_has_kompetanse_kompetanse_id",
                table: "fagområde_has_kompetanse",
                column: "kompetanse_id");

            migrationBuilder.CreateIndex(
                name: "IX_under_kompetanse_kompetanse_id",
                table: "under_kompetanse",
                column: "kompetanse_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fagområde_has_kompetanse");

            migrationBuilder.DropTable(
                name: "under_kompetanse");

            migrationBuilder.DropTable(
                name: "Fagområde");

            migrationBuilder.DropTable(
                name: "kompetanse");

            migrationBuilder.DropTable(
                name: "bedrift");
        }
    }
}
