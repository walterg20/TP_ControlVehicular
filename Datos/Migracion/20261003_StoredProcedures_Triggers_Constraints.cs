using Microsoft.EntityFrameworkCore.Migrations;

namespace TP_ControlVehicular.Datos.Migracion
{
    public partial class StoredProcedures_Triggers_Constraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE Servicios ADD CONSTRAINT CK_Servicio_Precio CHECK (Precio > 0);
                ALTER TABLE DetalleServicios ADD CONSTRAINT CK_DetalleServicio_Precio CHECK (Precio >= 0);
                ALTER TABLE DetalleServicios ADD CONSTRAINT CK_DetalleServicio_Cantidad CHECK (Cantidad > 0);
                ALTER TABLE RegistroServicios ADD CONSTRAINT CK_RegistroServicio_Total CHECK (Total >= 0);
                -- Asumiendo campos standard, ajustar de ser necesario
                ALTER TABLE RegistroServicios ADD CONSTRAINT CK_RegistroServicio_KmIngreso CHECK (Kilometraje >= 0);
                ALTER TABLE Vehiculos ADD CONSTRAINT CK_Vehiculo_KmActual CHECK (Kilometraje >= 0);
            ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER TR_DetalleServicio_ActualizarTotal
                ON DetalleServicios
                AFTER INSERT, UPDATE, DELETE
                AS
                BEGIN
                    UPDATE rs
                    SET rs.Total = (
                        SELECT ISNULL(SUM(ds.Precio * ds.Cantidad), 0)
                        FROM DetalleServicios ds
                        WHERE ds.RegistroServicioId = rs.Id
                    )
                    FROM RegistroServicios rs
                    WHERE rs.Id IN (
                        SELECT RegistroServicioId FROM inserted
                        UNION
                        SELECT RegistroServicioId FROM deleted
                    );
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER TR_DetalleServicio_ValidarPrecioYCantidad
                ON DetalleServicios
                AFTER INSERT, UPDATE
                AS
                BEGIN
                    IF EXISTS (SELECT 1 FROM inserted WHERE Precio < 0 OR Cantidad <= 0)
                    BEGIN
                        RAISERROR('Precio y cantidad deben ser válidos', 16, 1);
                        ROLLBACK TRANSACTION;
                    END
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER TR_RegistroServicio_SincronizarKm
                ON RegistroServicios
                AFTER INSERT
                AS
                BEGIN
                    UPDATE v
                    SET v.Kilometraje = i.Kilometraje
                    FROM Vehiculos v
                    INNER JOIN inserted i ON v.Id = i.VehiculoId
                    WHERE i.Kilometraje >= v.Kilometraje;

                    IF EXISTS (
                        SELECT 1 
                        FROM inserted i
                        INNER JOIN Vehiculos v ON v.Id = i.VehiculoId
                        WHERE i.Kilometraje < v.Kilometraje
                    )
                    BEGIN
                        RAISERROR('El kilometraje de ingreso no puede ser menor al kilometraje actual del vehículo.', 16, 1);
                        ROLLBACK TRANSACTION;
                    END
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER TR_DetalleServicio_BloquearModificacionFinalizada
                ON DetalleServicios
                AFTER INSERT, UPDATE, DELETE
                AS
                BEGIN
                    IF EXISTS (
                        SELECT 1 
                        FROM RegistroServicios rs
                        INNER JOIN Facturas f ON rs.Id = f.RegistroServicioId
                        WHERE rs.Id IN (SELECT RegistroServicioId FROM inserted UNION SELECT RegistroServicioId FROM deleted)
                    )
                    BEGIN
                        RAISERROR('No se pueden modificar detalles de una orden ya facturada.', 16, 1);
                        ROLLBACK TRANSACTION;
                    END
                END;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE Servicios DROP CONSTRAINT CK_Servicio_Precio;
                ALTER TABLE DetalleServicios DROP CONSTRAINT CK_DetalleServicio_Precio;
                ALTER TABLE DetalleServicios DROP CONSTRAINT CK_DetalleServicio_Cantidad;
                ALTER TABLE RegistroServicios DROP CONSTRAINT CK_RegistroServicio_Total;
                ALTER TABLE RegistroServicios DROP CONSTRAINT CK_RegistroServicio_KmIngreso;
                ALTER TABLE Vehiculos DROP CONSTRAINT CK_Vehiculo_KmActual;
            ");

            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS TR_DetalleServicio_ActualizarTotal;
                DROP TRIGGER IF EXISTS TR_DetalleServicio_ValidarPrecioYCantidad;
                DROP TRIGGER IF EXISTS TR_RegistroServicio_SincronizarKm;
                DROP TRIGGER IF EXISTS TR_DetalleServicio_BloquearModificacionFinalizada;
            ");
        }
    }
}
