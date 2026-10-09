USE [ControlVehicular];
GO


                CREATE OR ALTER PROCEDURE sp_ReporteIngresos
                    @StartDate DATETIME,
                    @EndDate DATETIME,
                    @WorkshopId INT = NULL
                AS
                BEGIN
                    SELECT 
                        CAST(f.Fecha AS DATE) AS Date,
                        t.Nombre AS WorkshopName,
                        SUM(f.Total) AS TotalRevenue
                    FROM Factura f
                    INNER JOIN RegistroServicio rs ON f.RegistroServicioId = rs.Id
                    INNER JOIN Taller t ON rs.TallerId = t.Id
                    WHERE f.Fecha >= @StartDate AND f.Fecha <= @EndDate
                      AND (@WorkshopId IS NULL OR t.Id = @WorkshopId)
                    GROUP BY CAST(f.Fecha AS DATE), t.Nombre
                    ORDER BY CAST(f.Fecha AS DATE) DESC;
                END;
            
GO


                CREATE OR ALTER PROCEDURE sp_ReporteTiemposResolucion
                    @StartDate DATETIME,
                    @EndDate DATETIME,
                    @WorkshopId INT = NULL
                AS
                BEGIN
                    SELECT 
                        t.Nombre AS WorkshopName,
                        AVG(CAST(DATEDIFF(HOUR, rs.Fecha, f.Fecha) AS FLOAT) / 24.0) AS AverageResolutionTimeDays
                    FROM RegistroServicio rs
                    INNER JOIN Factura f ON rs.Id = f.RegistroServicioId
                    INNER JOIN Taller t ON rs.TallerId = t.Id
                    WHERE rs.Fecha >= @StartDate AND rs.Fecha <= @EndDate
                      AND (@WorkshopId IS NULL OR t.Id = @WorkshopId)
                    GROUP BY t.Nombre
                    ORDER BY t.Nombre;
                END;
            
GO


                CREATE OR ALTER PROCEDURE sp_HistorialVehiculo
                    @VehiculoId INT = NULL,
                    @Patente NVARCHAR(50) = NULL
                AS
                BEGIN
                    SELECT 
                        v.Id AS VehiculoId,
                        v.Patente,
                        ma.NombreMarca AS Marca,
                        mo.NombreModelo AS Modelo,
                        rs.Fecha AS FechaIngreso,
                        f.Fecha AS FechaEgreso,
                        rs.Estado,
                        
                        ISNULL(f.Total, 0) AS MontoTotal
                    FROM Vehiculo v
                    INNER JOIN Modelo mo ON v.ModeloId = mo.Id
                    INNER JOIN Marca ma ON mo.MarcaId = ma.Id
                    INNER JOIN RegistroServicio rs ON v.Id = rs.VehiculoId
                    LEFT JOIN Factura f ON rs.Id = f.RegistroServicioId
                    WHERE (@VehiculoId IS NULL OR v.Id = @VehiculoId)
                      AND (@Patente IS NULL OR v.Patente = @Patente)
                    ORDER BY rs.Fecha DESC;
                END;
            
GO


                CREATE OR ALTER PROCEDURE sp_VehiculosPorCliente
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
                        ma.NombreMarca AS Marca,
                        mo.NombreModelo AS Modelo,
                        COUNT(rs.Id) AS CantidadServicio
                    FROM Cliente c
                    INNER JOIN PropietarioVehiculo pv ON c.Id = pv.ClienteId AND pv.EsActual = 1
                    INNER JOIN Vehiculo v ON pv.VehiculoId = v.Id
                    INNER JOIN Modelo mo ON v.ModeloId = mo.Id
                    INNER JOIN Marca ma ON mo.MarcaId = ma.Id
                    LEFT JOIN RegistroServicio rs ON v.Id = rs.VehiculoId
                    WHERE (@ClienteId IS NULL OR c.Id = @ClienteId)
                      AND (@DNI IS NULL OR c.Dni = @DNI)
                    GROUP BY c.Id, c.Nombre, c.Apellido, c.Dni, v.Id, v.Patente, ma.NombreMarca, mo.NombreModelo
                    ORDER BY c.Apellido, c.Nombre;
                END;
            
GO


                CREATE OR ALTER PROCEDURE sp_ServiciosPorMecanico
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
                    FROM DetalleServicio ds
                    INNER JOIN RegistroServicio rs ON ds.RegistroServicioId = rs.Id
                    INNER JOIN Vehiculo v ON rs.VehiculoId = v.Id
                    INNER JOIN Servicio s ON ds.ServicioId = s.Id
                    WHERE rs.Fecha >= @FechaDesde AND rs.Fecha <= @FechaHasta
                      AND ds.UsuarioId = @MecanicoId
                    ORDER BY rs.Fecha DESC;
                END;
            
GO


                ALTER TABLE Servicio ADD CONSTRAINT CK_Servicio_Precio CHECK (Precio > 0);
                ALTER TABLE DetalleServicio ADD CONSTRAINT CK_DetalleServicio_Precio CHECK (Precio >= 0);
                ALTER TABLE DetalleServicio ADD CONSTRAINT CK_DetalleServicio_Cantidad CHECK (Cantidad > 0);
                
                -- Asumiendo campos standard, ajustar de ser necesario
                ALTER TABLE RegistroServicio ADD CONSTRAINT CK_RegistroServicio_KmIngreso CHECK (KmIngreso >= 0);
                ALTER TABLE Vehiculo ADD CONSTRAINT CK_Vehiculo_KmActual CHECK (KmActual >= 0);
            
GO


                
            
GO


                CREATE OR ALTER TRIGGER TR_DetalleServicio_ValidarPrecioYCantidad
                ON DetalleServicio
                AFTER INSERT, UPDATE
                AS
                BEGIN
                    IF EXISTS (SELECT 1 FROM inserted WHERE Precio < 0 OR Cantidad <= 0)
                    BEGIN
                        RAISERROR('Precio y cantidad deben ser válidos', 16, 1);
                        ROLLBACK TRANSACTION;
                    END
                END;
            
GO


                CREATE OR ALTER TRIGGER TR_RegistroServicio_SincronizarKm
                ON RegistroServicio
                AFTER INSERT
                AS
                BEGIN
                    UPDATE v
                    SET v.KmActual = i.KmIngreso
                    FROM Vehiculo v
                    INNER JOIN inserted i ON v.Id = i.VehiculoId
                    WHERE i.KmIngreso >= v.KmActual;

                    DECLARE @ErrorMsg NVARCHAR(200);
                    DECLARE @KmIngreso INT;
                    DECLARE @KmActual INT;

                    SELECT TOP 1 @KmIngreso = i.KmIngreso, @KmActual = v.KmActual
                    FROM inserted i
                    INNER JOIN Vehiculo v ON v.Id = i.VehiculoId
                    WHERE i.KmIngreso < v.KmActual;

                    IF @KmActual IS NOT NULL
                    BEGIN
                        SET @ErrorMsg = CONCAT('El kilometraje de ingreso (', @KmIngreso, ') no puede ser menor al actual del vehículo (', @KmActual, ').');
                        RAISERROR(@ErrorMsg, 16, 1);
                        ROLLBACK TRANSACTION;
                    END
                END;
            
GO


                CREATE OR ALTER TRIGGER TR_DetalleServicio_BloquearModificacionFinalizada
                ON DetalleServicio
                AFTER INSERT, UPDATE, DELETE
                AS
                BEGIN
                    IF EXISTS (
                        SELECT 1 
                        FROM RegistroServicio rs
                        INNER JOIN Factura f ON rs.Id = f.RegistroServicioId
                        WHERE rs.Id IN (SELECT RegistroServicioId FROM inserted UNION SELECT RegistroServicioId FROM deleted)
                    )
                    BEGIN
                        RAISERROR('No se pueden modificar detalles de una orden ya facturada.', 16, 1);
                        ROLLBACK TRANSACTION;
                    END
                END;
            
GO




