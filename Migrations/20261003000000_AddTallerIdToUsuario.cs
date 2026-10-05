using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP_ControlVehicular.Migrations
{
    /// <inheritdoc />
    public partial class AddTallerIdToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TallerId",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_TallerId",
                table: "Usuarios",
                column: "TallerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Talleres_TallerId",
                table: "Usuarios",
                column: "TallerId",
                principalTable: "Talleres",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Talleres_TallerId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_TallerId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TallerId",
                table: "Usuarios");
        }
    }
}
