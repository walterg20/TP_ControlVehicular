using Microsoft.EntityFrameworkCore.Migrations;

namespace TP_ControlVehicular.Datos.Migracion
{
    public partial class SPs_ReportesTaller : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ServiciosPorMecanico
                    @FechaDesde DATETIME,
                    @FechaHasta DATETIME,
                    @MecanicoId INT
                AS
                BEGIN
                    SELECT 
                        rs.Fecha AS Fecha,
                        v.Patente AS Patente,
                        s.Nombre AS Servicio,
                        ds.Cantidad AS Cantidad,
                        (ds.Precio * ds.Cantidad) AS Importe
                    FROM DetalleServicios ds
                    INNER JOIN RegistroServicios rs ON ds.RegistroServicioId = rs.Id
                    INNER JOIN Vehiculos v ON rs.VehiculoId = v.Id
                    INNER JOIN Servicios s ON ds.ServicioId = s.Id
                    WHERE rs.Fecha >= @FechaDesde AND rs.Fecha <= @FechaHasta
                      AND ds.UsuarioId = @MecanicoId
                    ORDER BY rs.Fecha DESC;
                END;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ServiciosPorMecanico;");
        }
    }
}
