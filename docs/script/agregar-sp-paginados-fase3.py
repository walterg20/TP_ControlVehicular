"""Agrega a bd/03_procedimientos.sql los procedimientos paginados de la Fase 3.

- sp_ReporteOrdenesPaginado: reporte de ordenes paginado (proyeccion aplanada).
- sp_HistorialVehiculoPaginado: historial de un vehiculo paginado.
- sp_VehiculosPorClientePaginado: vehiculos de un cliente paginado.

Convencion: @TamanoPagina = 0 devuelve el conjunto completo (lo usa la exportacion a PDF).
"""

import pathlib
import sys

BASE = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")
RUTA = BASE / "bd" / "03_procedimientos.sql"

SQL = """
-- ----------------------------------------------------------------------------
-- sp_ReporteOrdenesPaginado - reporte de ordenes con paginacion a nivel de BD.
-- Consumido por ReporteRepository.ObtenerReporteOrdenesPaginadoAsync.
-- @TamanoPagina = 0 devuelve el conjunto completo (lo usa la exportacion a PDF).
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_ReporteOrdenesPaginado
    @TextoBusqueda NVARCHAR(200) = NULL,
    @Estado NVARCHAR(50) = NULL,
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL,
    @MecanicoId INT = NULL,
    @Pagina INT = 1,
    @TamanoPagina INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @Pagina < 1 SET @Pagina = 1;
    IF @TamanoPagina < 0 SET @TamanoPagina = 0;

    DECLARE @Patron NVARCHAR(202) = CASE
        WHEN @TextoBusqueda IS NULL OR LTRIM(RTRIM(@TextoBusqueda)) = '' THEN NULL
        ELSE N'%' + LTRIM(RTRIM(@TextoBusqueda)) + N'%'
    END;

    DECLARE @Base TABLE (
        IdOrden int NOT NULL,
        FechaIngreso datetime2 NOT NULL,
        Patente nvarchar(50) NOT NULL,
        MarcaModelo nvarchar(200) NOT NULL,
        ClienteNombre nvarchar(200) NOT NULL,
        ClienteDni nvarchar(50) NOT NULL,
        KmIngresado int NOT NULL,
        ServiciosAplicados nvarchar(max) NOT NULL,
        MecanicoNombre nvarchar(100) NOT NULL,
        RecepcionistaNombre nvarchar(100) NOT NULL,
        Estado nvarchar(50) NOT NULL
    );

    INSERT INTO @Base
    SELECT
        rs.Id AS IdOrden,
        rs.Fecha AS FechaIngreso,
        CAST(ISNULL(v.Patente, 'N/A') AS nvarchar(50)) AS Patente,
        CAST(CASE
            WHEN mo.Id IS NULL OR ma.Id IS NULL THEN 'Marca/Modelo N/A'
            ELSE ISNULL(ma.NombreMarca, '') + ' - ' + ISNULL(mo.NombreModelo, '')
        END AS nvarchar(200)) AS MarcaModelo,
        CAST(CASE
            WHEN c.Id IS NULL THEN 'Cliente N/A'
            ELSE LTRIM(RTRIM(ISNULL(c.Nombre, '') + ' ' + ISNULL(c.Apellido, '')))
        END AS nvarchar(200)) AS ClienteNombre,
        CAST(ISNULL(c.Dni, 'N/A') AS nvarchar(50)) AS ClienteDni,
        rs.KmIngreso AS KmIngresado,
        CAST(ISNULL(srv.ServiciosAplicados, 'Sin servicios asignados') AS nvarchar(max)) AS ServiciosAplicados,
        CAST(ISNULL(mec.MecanicoNombre, 'Sin Mecánico') AS nvarchar(100)) AS MecanicoNombre,
        CAST(ISNULL(us.Nombre, 'Sin Recepcionista') AS nvarchar(100)) AS RecepcionistaNombre,
        CAST(rs.Estado AS nvarchar(50)) AS Estado
    FROM RegistroServicio rs
    LEFT JOIN Vehiculo v ON rs.VehiculoId = v.Id
    LEFT JOIN Modelo mo ON v.ModeloId = mo.Id
    LEFT JOIN Marca ma ON mo.MarcaId = ma.Id
    LEFT JOIN Usuarios us ON rs.UsuarioId = us.Id
    OUTER APPLY (
        SELECT TOP 1 pv.ClienteId
        FROM PropietarioVehiculo pv
        WHERE pv.VehiculoId = v.Id AND pv.EsActual = 1
        ORDER BY pv.Id
    ) prop
    LEFT JOIN Cliente c ON prop.ClienteId = c.Id
    OUTER APPLY (
        SELECT STRING_AGG(s.Nombre, ', ') AS ServiciosAplicados
        FROM DetalleServicio ds
        INNER JOIN Servicio s ON ds.ServicioId = s.Id
        WHERE ds.RegistroServicioId = rs.Id
    ) srv
    OUTER APPLY (
        SELECT TOP 1 u.Nombre AS MecanicoNombre
        FROM DetalleServicio ds
        INNER JOIN Usuarios u ON ds.UsuarioId = u.Id
        WHERE ds.RegistroServicioId = rs.Id
        ORDER BY ds.Id
    ) mec
    WHERE (@MecanicoId IS NULL
            OR EXISTS (SELECT 1 FROM DetalleServicio dsm
                       WHERE dsm.RegistroServicioId = rs.Id AND dsm.UsuarioId = @MecanicoId))
      AND (@Estado IS NULL OR @Estado = '' OR @Estado = 'Todos' OR rs.Estado = @Estado)
      AND (@FechaDesde IS NULL OR CAST(rs.Fecha AS DATE) >= @FechaDesde)
      AND (@FechaHasta IS NULL OR CAST(rs.Fecha AS DATE) <= @FechaHasta)
      AND (@Patron IS NULL
            OR v.Patente LIKE @Patron
            OR ma.NombreMarca LIKE @Patron
            OR mo.NombreModelo LIKE @Patron
            OR c.Nombre LIKE @Patron
            OR c.Apellido LIKE @Patron
            OR c.Dni LIKE @Patron
            OR us.Nombre LIKE @Patron
            OR rs.Estado LIKE @Patron
            OR CONVERT(NVARCHAR(10), rs.Id) LIKE @Patron
            OR EXISTS (SELECT 1 FROM DetalleServicio dsu
                       INNER JOIN Usuarios uu ON dsu.UsuarioId = uu.Id
                       WHERE dsu.RegistroServicioId = rs.Id AND uu.Nombre LIKE @Patron)
            OR EXISTS (SELECT 1 FROM DetalleServicio dss
                       INNER JOIN Servicio ss ON dss.ServicioId = ss.Id
                       WHERE dss.RegistroServicioId = rs.Id AND ss.Nombre LIKE @Patron));

    DECLARE @TotalRegistros INT = (SELECT COUNT(*) FROM @Base);

    IF @TamanoPagina > 0
    BEGIN
        SELECT b.*, @TotalRegistros AS TotalRegistros
        FROM @Base b
        ORDER BY b.FechaIngreso DESC, b.IdOrden DESC
        OFFSET (@Pagina - 1) * @TamanoPagina ROWS
        FETCH NEXT @TamanoPagina ROWS ONLY;
    END
    ELSE
    BEGIN
        SELECT b.*, @TotalRegistros AS TotalRegistros
        FROM @Base b
        ORDER BY b.FechaIngreso DESC, b.IdOrden DESC;
    END
END
GO

-- ----------------------------------------------------------------------------
-- sp_HistorialVehiculoPaginado - historial de un vehiculo con paginacion a nivel de BD.
-- Consumido por ReporteRepository.ObtenerHistorialVehiculoPaginadoAsync.
-- @TamanoPagina = 0 devuelve el conjunto completo.
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_HistorialVehiculoPaginado
    @VehiculoId INT = NULL,
    @Patente NVARCHAR(50) = NULL,
    @Pagina INT = 1,
    @TamanoPagina INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @Pagina < 1 SET @Pagina = 1;
    IF @TamanoPagina < 0 SET @TamanoPagina = 0;

    DECLARE @Base TABLE (
        VehiculoId int NOT NULL,
        Patente nvarchar(50) NOT NULL,
        Marca nvarchar(100) NOT NULL,
        Modelo nvarchar(100) NOT NULL,
        FechaIngreso datetime2 NOT NULL,
        FechaEgreso datetime2 NULL,
        Estado nvarchar(50) NOT NULL,
        MontoTotal decimal(18,2) NOT NULL
    );

    INSERT INTO @Base
    SELECT
        v.Id AS VehiculoId,
        CAST(v.Patente AS nvarchar(50)) AS Patente,
        CAST(ma.NombreMarca AS nvarchar(100)) AS Marca,
        CAST(mo.NombreModelo AS nvarchar(100)) AS Modelo,
        rs.Fecha AS FechaIngreso,
        f.Fecha AS FechaEgreso,
        CAST(rs.Estado AS nvarchar(50)) AS Estado,
        CAST(ISNULL(f.Total, 0) AS decimal(18,2)) AS MontoTotal
    FROM Vehiculo v
    INNER JOIN Modelo mo ON v.ModeloId = mo.Id
    INNER JOIN Marca ma ON mo.MarcaId = ma.Id
    INNER JOIN RegistroServicio rs ON v.Id = rs.VehiculoId
    LEFT JOIN Factura f ON rs.Id = f.RegistroServicioId
    WHERE (@VehiculoId IS NULL OR v.Id = @VehiculoId)
      AND (@Patente IS NULL OR v.Patente = @Patente);

    DECLARE @TotalRegistros INT = (SELECT COUNT(*) FROM @Base);

    IF @TamanoPagina > 0
    BEGIN
        SELECT b.*, @TotalRegistros AS TotalRegistros
        FROM @Base b
        ORDER BY b.FechaIngreso DESC
        OFFSET (@Pagina - 1) * @TamanoPagina ROWS
        FETCH NEXT @TamanoPagina ROWS ONLY;
    END
    ELSE
    BEGIN
        SELECT b.*, @TotalRegistros AS TotalRegistros
        FROM @Base b
        ORDER BY b.FechaIngreso DESC;
    END
END
GO

-- ----------------------------------------------------------------------------
-- sp_VehiculosPorClientePaginado - vehiculos actuales de un cliente con conteo de
-- servicios, con paginacion a nivel de BD.
-- Consumido por ReporteRepository.ObtenerVehiculosPorClientePaginadoAsync.
-- @TamanoPagina = 0 devuelve el conjunto completo.
-- ----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_VehiculosPorClientePaginado
    @ClienteId INT = NULL,
    @DNI NVARCHAR(50) = NULL,
    @Pagina INT = 1,
    @TamanoPagina INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @Pagina < 1 SET @Pagina = 1;
    IF @TamanoPagina < 0 SET @TamanoPagina = 0;

    DECLARE @Base TABLE (
        ClienteId int NOT NULL,
        NombreCliente nvarchar(100) NOT NULL,
        ApellidoCliente nvarchar(100) NOT NULL,
        Dni nvarchar(50) NOT NULL,
        VehiculoId int NOT NULL,
        Patente nvarchar(50) NOT NULL,
        Marca nvarchar(100) NOT NULL,
        Modelo nvarchar(100) NOT NULL,
        CantidadServicio int NOT NULL
    );

    INSERT INTO @Base
    SELECT
        c.Id AS ClienteId,
        CAST(c.Nombre AS nvarchar(100)) AS NombreCliente,
        CAST(c.Apellido AS nvarchar(100)) AS ApellidoCliente,
        CAST(c.Dni AS nvarchar(50)) AS Dni,
        v.Id AS VehiculoId,
        CAST(v.Patente AS nvarchar(50)) AS Patente,
        CAST(ma.NombreMarca AS nvarchar(100)) AS Marca,
        CAST(mo.NombreModelo AS nvarchar(100)) AS Modelo,
        COUNT(rs.Id) AS CantidadServicio
    FROM Cliente c
    INNER JOIN PropietarioVehiculo pv ON c.Id = pv.ClienteId AND pv.EsActual = 1
    INNER JOIN Vehiculo v ON pv.VehiculoId = v.Id
    INNER JOIN Modelo mo ON v.ModeloId = mo.Id
    INNER JOIN Marca ma ON mo.MarcaId = ma.Id
    LEFT JOIN RegistroServicio rs ON v.Id = rs.VehiculoId
    WHERE (@ClienteId IS NULL OR c.Id = @ClienteId)
      AND (@DNI IS NULL OR c.Dni = @DNI)
    GROUP BY c.Id, c.Nombre, c.Apellido, c.Dni, v.Id, v.Patente, ma.NombreMarca, mo.NombreModelo;

    DECLARE @TotalRegistros INT = (SELECT COUNT(*) FROM @Base);

    IF @TamanoPagina > 0
    BEGIN
        SELECT b.*, @TotalRegistros AS TotalRegistros
        FROM @Base b
        ORDER BY b.ApellidoCliente, b.NombreCliente
        OFFSET (@Pagina - 1) * @TamanoPagina ROWS
        FETCH NEXT @TamanoPagina ROWS ONLY;
    END
    ELSE
    BEGIN
        SELECT b.*, @TotalRegistros AS TotalRegistros
        FROM @Base b
        ORDER BY b.ApellidoCliente, b.NombreCliente;
    END
END
GO
"""


def main() -> int:
    original = RUTA.read_text(encoding="utf-8")
    if "sp_ReporteOrdenesPaginado" in original:
        print("Los procedimientos paginados ya estan en el script canonico.")
        return 0

    separador = "" if original.endswith("\n") else "\n"
    RUTA.write_text(original + separador + SQL, encoding="utf-8", newline="")
    print("OK: procedimientos paginados agregados a bd/03_procedimientos.sql")
    return 0


if __name__ == "__main__":
    sys.exit(main())
