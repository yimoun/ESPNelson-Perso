using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StationnementAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaiementModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "Paiement");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UtilisateurId",
                table: "Paiement",
                type: "int",
                nullable: true);
        }
    }
}
