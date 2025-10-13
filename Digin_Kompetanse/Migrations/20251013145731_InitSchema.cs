using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Digin_Kompetanse.Migrations
{
    /// <inheritdoc />
    public partial class InitSchema : Migration
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
                name: "fagomrade",
                columns: table => new
                {
                    fagomrade_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fagomrade_navn = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    BedriftId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("fagomrade_pkey", x => x.fagomrade_id);
                    table.ForeignKey(
                        name: "FK_fagomrade_bedrift_BedriftId",
                        column: x => x.BedriftId,
                        principalTable: "bedrift",
                        principalColumn: "bedrift_id");
                });

            migrationBuilder.CreateTable(
                name: "login_token",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bedrift_id = table.Column<int>(type: "integer", nullable: false),
                    code_hash = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    consumed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("login_token_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_login_token_bedrift",
                        column: x => x.bedrift_id,
                        principalTable: "bedrift",
                        principalColumn: "bedrift_id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "fagomrade_has_kompetanse",
                columns: table => new
                {
                    fagomrade_id = table.Column<int>(type: "integer", nullable: false),
                    kompetanse_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("fagomrade_has_kompetanse_pkey", x => new { x.fagomrade_id, x.kompetanse_id });
                    table.ForeignKey(
                        name: "fagomrade_has_kompetanse_fagomrade_id_fkey",
                        column: x => x.fagomrade_id,
                        principalTable: "fagomrade",
                        principalColumn: "fagomrade_id");
                    table.ForeignKey(
                        name: "fagomrade_has_kompetanse_kompetanse_id_fkey",
                        column: x => x.kompetanse_id,
                        principalTable: "kompetanse",
                        principalColumn: "kompetanse_id");
                });

            migrationBuilder.CreateTable(
                name: "bedrift_kompetanse",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bedrift_id = table.Column<int>(type: "integer", nullable: false),
                    fagomrade_id = table.Column<int>(type: "integer", nullable: false),
                    kompetanse_id = table.Column<int>(type: "integer", nullable: false),
                    underkompetanse_id = table.Column<int>(type: "integer", nullable: true),
                    beskrivelse = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("bedrift_kompetanse_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_bedrift_kompetanse_bedrift",
                        column: x => x.bedrift_id,
                        principalTable: "bedrift",
                        principalColumn: "bedrift_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_bedrift_kompetanse_fagomrade",
                        column: x => x.fagomrade_id,
                        principalTable: "fagomrade",
                        principalColumn: "fagomrade_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_bedrift_kompetanse_kompetanse",
                        column: x => x.kompetanse_id,
                        principalTable: "kompetanse",
                        principalColumn: "kompetanse_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_bedrift_kompetanse_underkompetanse",
                        column: x => x.underkompetanse_id,
                        principalTable: "under_kompetanse",
                        principalColumn: "underkompetanse_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bedrift_kompetanse_bedrift_id",
                table: "bedrift_kompetanse",
                column: "bedrift_id");

            migrationBuilder.CreateIndex(
                name: "ix_bedrift_kompetanse_fagomrade_id",
                table: "bedrift_kompetanse",
                column: "fagomrade_id");

            migrationBuilder.CreateIndex(
                name: "ix_bedrift_kompetanse_kompetanse_id",
                table: "bedrift_kompetanse",
                column: "kompetanse_id");

            migrationBuilder.CreateIndex(
                name: "ix_bedrift_kompetanse_underkompetanse_id",
                table: "bedrift_kompetanse",
                column: "underkompetanse_id");

            migrationBuilder.CreateIndex(
                name: "ux_bedrift_kompetanse_unique_choice",
                table: "bedrift_kompetanse",
                columns: new[] { "bedrift_id", "fagomrade_id", "kompetanse_id", "underkompetanse_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_fagomrade_BedriftId",
                table: "fagomrade",
                column: "BedriftId");

            migrationBuilder.CreateIndex(
                name: "IX_fagomrade_has_kompetanse_kompetanse_id",
                table: "fagomrade_has_kompetanse",
                column: "kompetanse_id");

            migrationBuilder.CreateIndex(
                name: "ix_login_token_bedrift_expires",
                table: "login_token",
                columns: new[] { "bedrift_id", "expires_at" });

            migrationBuilder.CreateIndex(
                name: "IX_under_kompetanse_kompetanse_id",
                table: "under_kompetanse",
                column: "kompetanse_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bedrift_kompetanse");

            migrationBuilder.DropTable(
                name: "fagomrade_has_kompetanse");

            migrationBuilder.DropTable(
                name: "login_token");

            migrationBuilder.DropTable(
                name: "under_kompetanse");

            migrationBuilder.DropTable(
                name: "fagomrade");

            migrationBuilder.DropTable(
                name: "kompetanse");

            migrationBuilder.DropTable(
                name: "bedrift");
        }
    }
}
