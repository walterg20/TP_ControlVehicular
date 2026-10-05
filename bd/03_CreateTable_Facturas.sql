CREATE TABLE Facturas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RegistroServicioId INT NOT NULL,
    Total DECIMAL(18,2) NOT NULL,
    MontoRecibido DECIMAL(18,2) NOT NULL,
    MetodoPago NVARCHAR(50) NOT NULL,
    Fecha DATETIME2 NOT NULL,
    CONSTRAINT FK_Facturas_RegistroServicio FOREIGN KEY (RegistroServicioId) REFERENCES RegistroServicio(Id)
);
