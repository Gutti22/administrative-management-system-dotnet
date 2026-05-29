using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.Infrastructure.Repositories;

using MySqlConnector;
using SistemaAdmin.core.DTOs;
using SistemaAdmin.Core.Interfaces;
using SistemasAdmin.Infrastructure.Data;

public class ReporteRepository : IReporteRepository
{
    private readonly DbConnectionFactory _factory;

    public ReporteRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<ProductoMasVendidoDto>> TopProductosVendidosAsync()
    {
        const string sql = """
            SELECT p.Nombre, p.Codigo,
                   SUM(fd.Cantidad)  AS TotalUnidadesVendidas,
                   SUM(fd.Subtotal)  AS TotalFacturado
            FROM FacturaDetalles fd
            INNER JOIN Productos p ON p.Id = fd.ProductoId
            GROUP BY p.Id, p.Nombre, p.Codigo
            ORDER BY TotalUnidadesVendidas DESC
            LIMIT 5
            """;

        var resultado = new List<ProductoMasVendidoDto>();

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            resultado.Add(new ProductoMasVendidoDto
            {
                Nombre = reader.GetString("Nombre"),
                Codigo = reader.GetString("Codigo"),
                TotalUnidadesVendidas = reader.GetInt32("TotalUnidadesVendidas"),
                TotalFacturado = reader.GetDecimal("TotalFacturado")
            });
        }

        return resultado;
    }

    public async Task<IEnumerable<ClienteTopDto>> ClientesConMayorFacturacionAsync()
    {
        const string sql = """
            SELECT c.Nombre, c.Identidad, c.Correo,
                   COUNT(f.Id)   AS TotalFacturas,
                   SUM(f.Total)  AS MontoTotal
            FROM Facturas f
            INNER JOIN Clientes c ON c.Id = f.ClienteId
            GROUP BY c.Id, c.Nombre, c.Identidad, c.Correo
            ORDER BY MontoTotal DESC
            LIMIT 10
            """;

        var resultado = new List<ClienteTopDto>();

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            resultado.Add(new ClienteTopDto
            {
                Nombre = reader.GetString("Nombre"),
                Identidad = reader.GetString("Identidad"),
                Correo = reader.IsDBNull(reader.GetOrdinal("Correo")) ? null : reader.GetString("Correo"),
                TotalFacturas = reader.GetInt32("TotalFacturas"),
                MontoTotal = reader.GetDecimal("MontoTotal")
            });
        }

        return resultado;
    }

    public async Task<IEnumerable<ProductoStockBajoDto>> ProductosConStockBajoAsync()
    {
        const string sql = """
            SELECT Id, Nombre, Codigo, Stock, Precio
            FROM Productos
            WHERE Stock < 5 AND Estado = 1
            ORDER BY Stock ASC
            """;

        var resultado = new List<ProductoStockBajoDto>();

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            resultado.Add(new ProductoStockBajoDto
            {
                Id = reader.GetInt32("Id"),
                Nombre = reader.GetString("Nombre"),
                Codigo = reader.GetString("Codigo"),
                Stock = reader.GetInt32("Stock"),
                Precio = reader.GetDecimal("Precio")
            });
        }

        return resultado;
    }
}
