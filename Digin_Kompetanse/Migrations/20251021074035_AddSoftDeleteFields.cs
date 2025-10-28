using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
// liten endring for å trigge git push
namespace Digin_Kompetanse.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_bedrift_kompetanse_unique_choice",
                table: "bedrift_kompetanse");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "bedrift_kompetanse",
                type: "boolean",
                nullable: false, 
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "bedrift_kompetanse",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "modified_by_bedrift_id",
                table: "bedrift_kompetanse",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ux_bedrift_kompetanse_unique_choice_active",
                table: "bedrift_kompetanse",
                columns: new[] { "bedrift_id", "fagomrade_id", "kompetanse_id", "underkompetanse_id" },
                unique: true,
                filter: "\"is_active\" = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_bedrift_kompetanse_unique_choice_active",
                table: "bedrift_kompetanse");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "bedrift_kompetanse");

            migrationBuilder.DropColumn(
                name: "modified_at",
                table: "bedrift_kompetanse");

            migrationBuilder.DropColumn(
                name: "modified_by_bedrift_id",
                table: "bedrift_kompetanse");

            migrationBuilder.CreateIndex(
                name: "ux_bedrift_kompetanse_unique_choice",
                table: "bedrift_kompetanse",
                columns: new[] { "bedrift_id", "fagomrade_id", "kompetanse_id", "underkompetanse_id" },
                unique: true);
        }
    }
}
