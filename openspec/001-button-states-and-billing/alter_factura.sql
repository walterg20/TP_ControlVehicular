ALTER TABLE Factura DROP COLUMN Numero;
ALTER TABLE Factura ADD Numero INT IDENTITY(1,1) NOT NULL;
ALTER TABLE Factura ADD CONSTRAINT UQ_Factura_RegistroServicioId UNIQUE (RegistroServicioId);
