-- Tabla: METODO_PAGO
CREATE TABLE MetodoPago (
    id INT IDENTITY(1,1),
    nombre VARCHAR(50) NOT NULL,
    CONSTRAINT pk_metodo_pago PRIMARY KEY (id)
);

-- Tabla: FACTURA
CREATE TABLE Factura (
    id_factura INT IDENTITY(1,1),
    id_servicio INT NOT NULL,
    numero int NOT NULL,
    fecha DATE NOT NULL,
    total DECIMAL(10, 2) NOT NULL,
    CONSTRAINT pk_factura PRIMARY KEY (id_factura),
    CONSTRAINT fk_factura_servicio FOREIGN KEY (id_servicio) 
        REFERENCES RegistroServicio(id)
);

-- Tabla: PAGO
CREATE TABLE Pago (
    id_pago INT IDENTITY(1,1),
    id_factura INT NOT NULL,
    id_metodo_pago INT NOT NULL,
    fecha DATE NOT NULL,
    monto DECIMAL(10, 2) NOT NULL,
    CONSTRAINT pk_pago PRIMARY KEY (id_pago),
    CONSTRAINT fk_pago_factura FOREIGN KEY (id_factura) 
        REFERENCES FACTURA(id_factura),
    CONSTRAINT fk_pago_metodo_pago FOREIGN KEY (id_metodo_pago) 
        REFERENCES MetodoPago(id)
);