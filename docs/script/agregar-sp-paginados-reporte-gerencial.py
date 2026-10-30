"""Agrega los procedimientos paginados de las pestanas del Reporte Gerencial.

- sp_ReporteTopClientesPaginado
- sp_ReporteModelosReparadosPaginado
- sp_ReporteServiciosPorRangoPaginado

A diferencia de los SP originales, estos NO aplican el tope TOP (@TopN): devuelven el
ranking completo y lo paginan en la base de datos. Con @TamanoPagina = 0 devuelven el
conjunto completo (lo usa la exportacion a PDF).
"""

import pathlib
import sys

BASE = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")
RUTA = BASE / "bd" / "03_procedimientos.sql"

SQL = """
-- ----------------------------------------------------------------------------
-- sp_ReporteTopClientesPaginado - ranking de clientes con paginacion a nivel de BD.
-- Consumido por ReporteGerencialRepository.ObtenerTopClientesPaginadoAsync.
-- A diferencia de sp_ReporteTopClientes no aplica el tope TOP (@TopN): devuelve el
-- ranking completo y lo pagina. Con @TamanoPagina = 0 devuelve el conjunto completo.
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_ReporteTopClientesPaginado
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL,
    @Pagina INT = 1,
    @TamanoPagina INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @Pagina < 1 SET @Pagina = 1;
    IF @TamanoPagina < 0 SET @TamanoPagina = 0;

    DECLARE @Base TABLE (
        ClienteId int NOT NULL,
        Nombre nvarchar(100) NOT NULL,
        Apellido nvarchar(100) NOT NULL,
        Dni nvarchar(50) NOT NULL,
        CantidadServicios int NOT NULL,
        TotalGastado decimal(18,2) NOT NULL
    );

    INSERT INTO @Base
    SELECT
        c.Id AS ClienteId,
        CAST(c.Nombre AS nvarchar(100)) AS Nombre,
        CAST(c.Apellido AS nvarchar(100)) AS Apellido,
        CAST(c.Dni AS nvarchar(50)) AS Dni,
        COUNT(rs.Id) AS CantidadServicios,
        CAST(ISNULL(SUM(f.Total), 0) AS decimal(18,2)) AS TotalGastado
    FROM Cliente c
    INNER JOIN PropietarioVehiculo pv ON c.Id = pv.ClienteId AND pv.EsActual = 1
    INNER JOIN Vehiculo v ON pv.VehiculoId = v.Id
    INNER JOIN RegistroServicio rs ON v.Id = rs.VehiculoId
    LEFT JOIN Factura f ON rs.Id = f.RegistroServicioId
    WHERE (@FechaDesde IS NULL OR CAST(rs.Fecha AS DATE) >= @FechaDesde)
      AND (@FechaHasta IS NULL OR CAST(rs.Fecha AS DATE) <= @FechaHasta)
    GROUP BY c.Id, c.Nombre, c.Apellido, c.Dni;

    DECLARE @TotalRegistros INT = (SELECT COUNT(*) FROM @Base);

    IF @TamanoPagina > 0
    BEGIN
        SELECT b.*, @TotalRegistros AS TotalRegistros
        FROM @Base b
        ORDER BY b.CantidadServicios DESC, b.TotalGastado DESC
        OFFSET (@Pagina - 1) * @TamanoPagina ROWS
        FETCH NEXT @TamanoPagina ROWS ONLY;
    END
    ELSE
    BEGIN
        SELECT b.*, @TotalRegistros AS TotalRegistros
        FROM @Base b
        ORDER BY b.CantidadServicios DESC, b.TotalGastado DESC;
    END
END
GO

-- ----------------------------------------------------------------------------
-- sp_ReporteModelosReparadosPaginado - ranking de modelos con paginacion a nivel de BD.
-- Consumido por ReporteGerencialRepository.ObtenerModelosMasReparadosPaginadoAsync.
-- No aplica el tope TOP (@TopN): devuelve el ranking completo y lo pagina.
-- Con @TamanoPagina = 0 devuelve el conjunto completo.
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_ReporteModelosReparadosPaginado
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL,
    @Pagina INT = 1,
    @TamanoPagina INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @Pagina < 1 SET @Pagina = 1;
    IF @TamanoPagina < 0 SET @TamanoPagina = 0;

    DECLARE @Base TABLE (
        Marca nvarchar(100) NOT NULL,
        Modelo nvarchar(100) NOT NULL,
        CantidadReparaciones int NOT NULL
    );

    INSERT INTO @Base
    SELECT
        CAST(ma.NombreMarca AS nvarchar(100)) AS Marca,
        CAST(mo.NombreModelo AS nvarchar(100)) AS Modelo,
        COUNT(rs.Id) AS CantidadReparaciones
    FROM RegistroServicio rs
    INNER JOIN Vehiculo v ON rs.VehiculoId = v.Id
    INNER JOIN Modelo mo ON v.ModeloId = mo.Id
    INNER JOIN Marca ma ON mo.MarcaId = ma.Id
    WHERE (@FechaDesde IS NULL OR CAST(rs.Fecha AS DATE) >= @FechaDesde)
      AND (@FechaHasta IS NULL OR CAST(rs.Fecha AS DATE) <= @FechaHasta)
    GROUP BY ma.NombreMarca, mo.NombreModelo;

    DECLARE @TotalRegistros INT = (SELECT COUNT(*) FROM @Base);

    IF @TamanoPagina > 0
    BEGIN
        SELECT b.*, @TotalRegistros AS TotalRegistros
        FROM @Base b
        ORDER BY b.CantidadReparaciones DESC, b.Marca, b.Modelo
        OFFSET (@Pagina - 1) * @TamanoPagina ROWS
        FETCH NEXT @TamanoPagina ROWS ONLY;
    END
    ELSE
    BEGIN
        SELECT b.*, @TotalRegistros AS TotalRegistros
        FROM @Base b
        ORDER BY b.CantidadReparaciones DESC, b.Marca, b.Modelo;
    END
END
GO

-- ----------------------------------------------------------------------------
-- sp_ReporteServiciosPorRangoPaginado - servicios por cantidad con paginacion a nivel de BD.
-- Consumido por ReporteGerencialRepository.ObtenerServiciosPorRangoPaginadoAsync.
-- Con @TamanoPagina = 0 devuelve el conjunto completo.
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_ReporteServiciosPorRangoPaginado
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL,
    @Pagina INT = 1,
    @TamanoPagina INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @Pagina < 1 SET @Pagina = 1;
    IF @TamanoPagina < 0 SET @TamanoPagina = 0;

    DECLARE @Base TABLE (
        ServicioId int NOT NULL,
        Servicio nvarchar(200) NOT NULL,
        Cantidad int NOT NULL
    );

    INSERT INTO @Base
    SELECT
        s.Id AS ServicioId,
        CAST(s.Nombre AS nvarchar(200)) AS Servicio,
        SUM(ds.Cantidad) AS Cantidad
    FROM DetalleServicio ds
    INNER JOIN RegistroServicio rs ON ds.RegistroServicioId = rs.Id
    INNER JOIN Servicio s ON ds.ServicioId = s.Id
    WHERE rs.Estado <> 'Cancelada'
      AND (@FechaDesde IS NULL OR CAST(rs.Fecha AS DATE) >= @FechaDesde)
      AND (@FechaHasta IS NULL OR CAST(rs.Fecha AS DATE) <= @FechaHasta)
    GROUP BY s.Id, s.Nombre
    HAVING SUM(ds.Cantidad) > 0;

    DECLARE @TotalRegistros INT = (SELECT COUNT(*) FROM @Base);

    IF @TamanoPagina > 0
    BEGIN
        SELECT b.*, @TotalRegistros AS TotalRegistros
        FROM @Base b
        ORDER BY b.Cantidad DESC, b.Servicio ASC
        OFFSET (@Pagina - 1) * @TamanoPagina ROWS
        FETCH NEXT @TamanoPagina ROWS ONLY;
    END
    ELSE
    BEGIN
        SELECT b.*, @TotalRegistros AS TotalRegistros
        FROM @Base b
        ORDER BY b.Cantidad DESC, b.Servicio ASC;
    END
END
GO
"""


def main() -> int:
    original = RUTA.read_text(encoding="utf-8")
    if "sp_ReporteTopClientesPaginado" in original:
        print("Los procedimientos paginados del reporte gerencial ya estan en el script canonico.")
        return 0

    separador = "" if original.endswith("\n") else "\n"
    RUTA.write_text(original + separador + SQL, encoding="utf-8", newline="")
    print("OK: procedimientos paginados del reporte gerencial agregados")
    return 0


if __name__ == "__main__":
    sys.exit(main())
