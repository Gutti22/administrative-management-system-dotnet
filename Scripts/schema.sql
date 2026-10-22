-- ============================================================
--  Sistema Administrativo - Script de creación de base de datos
--  Motor: MySQL 8.0+
-- ============================================================

CREATE DATABASE IF NOT EXISTS SistemaAdmin
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE SistemaAdmin;

-- ------------------------------------------------------------
--  Tabla Clientes
-- ------------------------------------------------------------
CREATE TABLE Clientes (
    Id            INT           NOT NULL AUTO_INCREMENT,
    Nombre        VARCHAR(150)  NOT NULL,
    Identidad      VARCHAR(20)   NOT NULL,
    Telefono      VARCHAR(20)       NULL,
    Correo        VARCHAR(100)      NULL,
    FechaRegistro DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Estado        TINYINT(1)    NOT NULL DEFAULT 1,

    CONSTRAINT PK_Clientes PRIMARY KEY (Id)
);

CREATE INDEX IX_Clientes_Nombre       ON Clientes (Nombre);
CREATE INDEX IX_Clientes_Identidad ON Clientes (Identidad);
CREATE INDEX IX_Clientes_Correo       ON Clientes (Correo);

-- ------------------------------------------------------------
--  Tabla Productos
-- ------------------------------------------------------------
CREATE TABLE Productos (
    Id      INT            NOT NULL AUTO_INCREMENT,
    Nombre  VARCHAR(150)   NOT NULL,
    Codigo  VARCHAR(50)    NOT NULL,
    Precio  DECIMAL(10,2)  NOT NULL,
    Stock   INT            NOT NULL DEFAULT 0,
    Estado  TINYINT(1)     NOT NULL DEFAULT 1,

    CONSTRAINT PK_Productos PRIMARY KEY (Id),
    CONSTRAINT UQ_Productos_Codigo UNIQUE (Codigo)
);

CREATE INDEX IX_Productos_Nombre ON Productos (Nombre);

-- ------------------------------------------------------------
--  Tabla Facturas
-- ------------------------------------------------------------
CREATE TABLE Facturas (
    Id             INT            NOT NULL AUTO_INCREMENT,
    NumeroFactura  VARCHAR(20)    NOT NULL,
    ClienteId      INT            NOT NULL,
    Fecha          DATETIME       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Subtotal       DECIMAL(10,2)  NOT NULL,
    ISV            DECIMAL(10,2)  NOT NULL,
    Total          DECIMAL(10,2)  NOT NULL,

    CONSTRAINT PK_Facturas      PRIMARY KEY (Id),
    CONSTRAINT UQ_NumeroFactura UNIQUE (NumeroFactura),
    CONSTRAINT FK_Facturas_Cliente
        FOREIGN KEY (ClienteId) REFERENCES Clientes (Id)
);

CREATE INDEX IX_Facturas_ClienteId ON Facturas (ClienteId);
CREATE INDEX IX_Facturas_Fecha     ON Facturas (Fecha);

-- ------------------------------------------------------------
--  Tabla Detalle de Facturas
-- ------------------------------------------------------------
CREATE TABLE FacturaDetalles (
    Id              INT            NOT NULL AUTO_INCREMENT,
    FacturaId       INT            NOT NULL,
    ProductoId      INT            NOT NULL,
    Cantidad        INT            NOT NULL,
    PrecioUnitario  DECIMAL(10,2)  NOT NULL,
    Subtotal        DECIMAL(10,2)  NOT NULL,

    CONSTRAINT PK_FacturaDetalles PRIMARY KEY (Id),
    CONSTRAINT FK_Detalle_Factura
        FOREIGN KEY (FacturaId)  REFERENCES Facturas  (Id),
    CONSTRAINT FK_Detalle_Producto
        FOREIGN KEY (ProductoId) REFERENCES Productos (Id)
);

CREATE INDEX IX_Detalle_FacturaId  ON FacturaDetalles (FacturaId);
CREATE INDEX IX_Detalle_ProductoId ON FacturaDetalles (ProductoId);

-- ------------------------------------------------------------
--  Tabla de Bitácora de errores
-- ------------------------------------------------------------
CREATE TABLE BitacoraErrores (
    Id         INT           NOT NULL AUTO_INCREMENT,
    Fecha      DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Nivel      VARCHAR(20)   NOT NULL,
    Mensaje    VARCHAR(500)  NOT NULL,
    StackTrace TEXT              NULL,

    CONSTRAINT PK_BitacoraErrores PRIMARY KEY (Id)
);

CREATE INDEX IX_Bitacora_Fecha ON BitacoraErrores (Fecha);
CREATE INDEX IX_Bitacora_Nivel ON BitacoraErrores (Nivel);