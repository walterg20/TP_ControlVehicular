using Microsoft.EntityFrameworkCore.Migrations;

namespace TP_ControlVehicular.Datos.Migracion
{
    public partial class SPs_ReportesRecepcion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_HistorialVehiculo
                    @VehiculoId INT = NULL,
                    @Patente NVARCHAR(50) = NULL
                AS
                BEGIN
                    SELECT 
                        v.Id AS VehiculoId,
                        v.Patente,
                        v.Marca,
                        v.Modelo,
                        rs.Fecha AS FechaIngreso,
                        f.Fecha AS FechaEgreso,
                        rs.Estado,
                        rs.Diagnostico,
                        ISNULL(f.Total, 0) AS MontoTotal
                    FROM Vehiculos v
                    INNER JOIN RegistroServicios rs ON v.Id = rs.VehiculoId
                    LEFT JOIN Facturas f ON rs.Id = f.RegistroServicioId
                    WHERE (@VehiculoId IS NULL OR v.Id = @VehiculoId)
                      AND (@Patente IS NULL OR v.Patente = @Patente)
                    ORDER BY rs.Fecha DESC;
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_VehiculosPorCliente
                    @ClienteId INT = NULL,
                    @DNI NVARCHAR(50) = NULL
                AS
                BEGIN
                    SELECT 
                        c.Id AS ClienteId,
                        c.Nombre AS NombreCliente,
                        c.Apellido AS ApellidoCliente,
                        c.Dni,
                        v.Id AS VehiculoId,
                        v.Patente,
                        v.Marca,
                        v.Modelo,
                        COUNT(rs.Id) AS CantidadServicios
                    FROM Clientes c
                    INNER JOIN Vehiculos v ON c.Id = v.ClienteId
                    LEFT JOIN RegistroServicios rs ON v.Id = rs.VehiculoId
                    WHERE (@ClienteId IS NULL OR c.Id = @ClienteId)
                      AND (@DNI IS NULL OR c.Dni = @DNI)
                    GROUP BY c.Id, c.Nombre, c.Apellido, c.Dni, v.Id, v.Patente, v.Marca, v.Modelo
                    ORDER BY c.Apellido, c.Nombre;
                END;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_HistorialVehiculo;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_VehiculosPorCliente;");
        }
    }
}
