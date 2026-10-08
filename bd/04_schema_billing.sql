-- schema_billing.sql (Corregido para SQL Server con DROP previos)

IF OBJECT_ID('Pago', 'U') IS NOT NULL DROP TABLE Pago;
IF OBJECT_ID('Factura', 'U') IS NOT NULL DROP TABLE Factura;
IF OBJECT_ID('MetodoPago', 'U') IS NOT NULL DROP TABLE MetodoPago;

-- En caso de que se hayan creado con el nombre anterior en mayúsculas
IF OBJECT_ID('PAGO', 'U') IS NOT NULL DROP TABLE PAGO;
IF OBJECT_ID('FACTURA', 'U') IS NOT NULL DROP TABLE FACTURA;
IF OBJECT_ID('METODO_PAGO', 'U') IS NOT NULL DROP TABLE METODO_PAGO;

-- 1. Create MetodoPago table
CREATE TABLE MetodoPago (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL
);

-- 2. Create Factura table
CREATE TABLE Factura (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RegistroServicioId INT NOT NULL,
    Numero INT NOT NULL UNIQUE, -- Numero consecutivo, no null y no repetido
    Fecha DATE NOT NULL,
    Total DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_Factura_Registro FOREIGN KEY (RegistroServicioId) REFERENCES RegistroServicio(Id)
);

-- 3. Create Pago table
CREATE TABLE Pago (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FacturaId INT NOT NULL,
    MetodoPagoId INT NOT NULL,
    Monto DECIMAL(10,2) NOT NULL,
    FechaPago DATE NOT NULL,
    CONSTRAINT FK_Pago_Factura FOREIGN KEY (FacturaId) REFERENCES Factura(Id),
    CONSTRAINT FK_Pago_Metodo FOREIGN KEY (MetodoPagoId) REFERENCES MetodoPago(Id)
);

-- Seed data for MetodoPago
INSERT INTO MetodoPago (Nombre) VALUES ('Efectivo');
INSERT INTO MetodoPago (Nombre) VALUES ('Tarjeta');
