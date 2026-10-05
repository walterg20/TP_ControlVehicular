using Microsoft.EntityFrameworkCore.Migrations;

namespace TP_ControlVehicular.Datos.Migracion
{
    public partial class SPs_ReportesGerenciales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ReporteIngresos
                    @StartDate DATETIME,
                    @EndDate DATETIME,
                    @WorkshopId INT = NULL
                AS
                BEGIN
                    SELECT 
                        CAST(f.Fecha AS DATE) AS Date,
                        t.Nombre AS WorkshopName,
                        SUM(f.Total) AS TotalRevenue
                    FROM Facturas f
                    INNER JOIN RegistroServicios rs ON f.RegistroServicioId = rs.Id
                    INNER JOIN Talleres t ON rs.TallerId = t.Id
                    WHERE f.Fecha >= @StartDate AND f.Fecha <= @EndDate
                      AND (@WorkshopId IS NULL OR t.Id = @WorkshopId)
                    GROUP BY CAST(f.Fecha AS DATE), t.Nombre
                    ORDER BY CAST(f.Fecha AS DATE) DESC;
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ReporteTiemposResolucion
                    @StartDate DATETIME,
                    @EndDate DATETIME,
                    @WorkshopId INT = NULL
                AS
                BEGIN
                    SELECT 
                        t.Nombre AS WorkshopName,
                        AVG(CAST(DATEDIFF(HOUR, rs.Fecha, f.Fecha) AS FLOAT) / 24.0) AS AverageResolutionTimeDays
                    FROM RegistroServicios rs
                    INNER JOIN Facturas f ON rs.Id = f.RegistroServicioId
                    INNER JOIN Talleres t ON rs.TallerId = t.Id
                    WHERE rs.Fecha >= @StartDate AND rs.Fecha <= @EndDate
                      AND (@WorkshopId IS NULL OR t.Id = @WorkshopId)
                    GROUP BY t.Nombre
                    ORDER BY t.Nombre;
                END;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ReporteIngresos;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ReporteTiemposResolucion;");
        }
    }
}
