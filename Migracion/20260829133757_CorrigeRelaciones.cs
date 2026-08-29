using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP_ControlVehicular.Migracion
{
    /// <inheritdoc />
    public partial class CorrigeRelaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modelo_Marca_IdMarca",
                table: "Modelo");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehiculo_Cliente_IdCliente",
                table: "Vehiculo");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehiculo_Modelo_IdModelo",
                table: "Vehiculo");

            migrationBuilder.RenameColumn(
                name: "IdModelo",
                table: "Vehiculo",
                newName: "ModeloId");

            migrationBuilder.RenameColumn(
                name: "IdCliente",
                table: "Vehiculo",
                newName: "ClienteId");

            migrationBuilder.RenameColumn(
                name: "IdVehiculo",
                table: "Vehiculo",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Vehiculo_IdModelo",
                table: "Vehiculo",
                newName: "IX_Vehiculo_ModeloId");

            migrationBuilder.RenameIndex(
                name: "IX_Vehiculo_IdCliente",
                table: "Vehiculo",
                newName: "IX_Vehiculo_ClienteId");

            migrationBuilder.RenameColumn(
                name: "IdMarca",
                table: "Modelo",
                newName: "MarcaId");

            migrationBuilder.RenameColumn(
                name: "IdModelo",
                table: "Modelo",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Modelo_IdMarca",
                table: "Modelo",
                newName: "IX_Modelo_MarcaId");

            migrationBuilder.RenameColumn(
                name: "IdMarca",
                table: "Marca",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdCliente",
                table: "Cliente",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Modelo_Marca_MarcaId",
                table: "Modelo",
                column: "MarcaId",
                principalTable: "Marca",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehiculo_Cliente_ClienteId",
                table: "Vehiculo",
                column: "ClienteId",
                principalTable: "Cliente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehiculo_Modelo_ModeloId",
                table: "Vehiculo",
                column: "ModeloId",
                principalTable: "Modelo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modelo_Marca_MarcaId",
                table: "Modelo");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehiculo_Cliente_ClienteId",
                table: "Vehiculo");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehiculo_Modelo_ModeloId",
                table: "Vehiculo");

            migrationBuilder.RenameColumn(
                name: "ModeloId",
                table: "Vehiculo",
                newName: "IdModelo");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "Vehiculo",
                newName: "IdCliente");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Vehiculo",
                newName: "IdVehiculo");

            migrationBuilder.RenameIndex(
                name: "IX_Vehiculo_ModeloId",
                table: "Vehiculo",
                newName: "IX_Vehiculo_IdModelo");

            migrationBuilder.RenameIndex(
                name: "IX_Vehiculo_ClienteId",
                table: "Vehiculo",
                newName: "IX_Vehiculo_IdCliente");

            migrationBuilder.RenameColumn(
                name: "MarcaId",
                table: "Modelo",
                newName: "IdMarca");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Modelo",
                newName: "IdModelo");

            migrationBuilder.RenameIndex(
                name: "IX_Modelo_MarcaId",
                table: "Modelo",
                newName: "IX_Modelo_IdMarca");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Marca",
                newName: "IdMarca");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Cliente",
                newName: "IdCliente");

            migrationBuilder.AddForeignKey(
                name: "FK_Modelo_Marca_IdMarca",
                table: "Modelo",
                column: "IdMarca",
                principalTable: "Marca",
                principalColumn: "IdMarca",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehiculo_Cliente_IdCliente",
                table: "Vehiculo",
                column: "IdCliente",
                principalTable: "Cliente",
                principalColumn: "IdCliente",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehiculo_Modelo_IdModelo",
                table: "Vehiculo",
                column: "IdModelo",
                principalTable: "Modelo",
                principalColumn: "IdModelo",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
