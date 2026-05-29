# Sistema Administrativo — Prueba Técnica

Sistema web de administración desarrollado con **Blazor Server**, **C# .NET 8** y **MySQL**, 
siguiendo una arquitectura limpia por capas.

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- MySQL Server 8.0+
- Visual Studio 2022

## Configuración inicial

### 1. Base de datos

Abre MySQL Workbench y ejecuta el script ubicado en:
/scripts/schema.sql

Esto creará la base de datos `SistemaAdmin` con todas las tablas necesarias.

### 2. Connection String

Abre `SistemaAdmin.Web/appsettings.json` y actualiza la contraseña:

```json
{
  "ConnectionStrings": {
    "MySql": "Server=localhost;Port=3306;Database=SistemaAdmin;Uid=root;Pwd=TU_PASSWORD;"
  }
}
```

### 3. Ejecutar el proyecto

Abre `SistemaAdmin.sln` en Visual Studio 2022, establece `SistemaAdmin.Web` 
como proyecto de inicio y presiona **F5**.

La aplicación abrirá en `https://localhost:{puerto}`

## Estructura del proyecto

SistemaAdmin/
??? SistemaAdmin.Core/           # Entidades e interfaces (sin dependencias)
??? SistemaAdmin.Infrastructure/ # ADO.NET, repositorios MySQL
??? SistemaAdmin.Application/    # Servicios de negocio, DTOs
??? SistemaAdmin.Web/            # Blazor Server (UI)

## Módulos disponibles

| Módulo | Ruta | Descripción |
|---|---|---|
| Clientes | `/clientes` | CRUD completo con búsqueda y activar/desactivar |
| Productos | `/productos` | CRUD completo con control de stock |
| Facturación | `/facturas` | Emisión de facturas con ISV 15% automático |
| Reportes | `/reportes` | 3 reportes SQL en tiempo real |
| Bitácora | `/bitacora` | Registro de errores del sistema |

## Decisiones técnicas destacadas

- **ADO.NET puro** sobre ORM para máximo control sobre las queries
- **Transacciones** en emisión de facturas para garantizar integridad
- **Arquitectura por capas** con dependencias unidireccionales
- **ISV calculado y persistido** para auditoría histórica correcta