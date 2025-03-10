using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StationnementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AssociatePaiementAndTarification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TarificationDureeMax",
                table: "Paiement",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TarificationDureeMin",
                table: "Paiement",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TarificationNiveau",
                table: "Paiement",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "TarificationPrix",
                table: "Paiement",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TarificationDureeMax",
                table: "Paiement");

            migrationBuilder.DropColumn(
                name: "TarificationDureeMin",
                table: "Paiement");

            migrationBuilder.DropColumn(
                name: "TarificationNiveau",
                table: "Paiement");

            migrationBuilder.DropColumn(
                name: "TarificationPrix",
                table: "Paiement");
        }
    }
}
