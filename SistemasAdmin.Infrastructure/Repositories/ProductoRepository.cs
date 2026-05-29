using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemasAdmin.Infrastructure.Repositories;

using MySqlConnector;
using SistemaAdmin.core.Entities;
using SistemaAdmin.core.Interfaces;
using SistemasAdmin.Infrastructure.Data;


public class ProductoRepository : IProductoRepository
{
    private readonly DbConnectionFactory _factory;

    public ProductoRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
    {
        const string sql = """
            SELECT Id, Nombre, Codigo, Precio, Stock, Estado
            FROM Productos
            ORDER BY Nombre
            """;

        return await EjecutarListaAsync(sql, null);
    }

    public async Task<IEnumerable<Producto>> ObtenerActivosAsync()
    {
        const string sql = """
            SELECT Id, Nombre, Codigo, Precio, Stock, Estado
            FROM Productos
            WHERE Estado = 1 AND Stock > 0
            ORDER BY Nombre
            """;

        return await EjecutarListaAsync(sql, null);
    }

    public async Task<Producto?> ObtenerPorIdAsync(int id)
    {
        const string sql = """
            SELECT Id, Nombre, Codigo, Precio, Stock, Estado
            FROM Productos
            WHERE Id = @Id
            """;

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapearProducto(reader) : null;
    }

    public async Task<int> CrearAsync(Producto producto)
    {
        const string sql = """
            INSERT INTO Productos (Nombre, Codigo, Precio, Stock)
            VALUES (@Nombre, @Codigo, @Precio, @Stock);
            SELECT LAST_INSERT_ID();
            """;

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
        cmd.Parameters.AddWithValue("@Codigo", producto.Codigo);
        cmd.Parameters.AddWithValue("@Precio", producto.Precio);
        cmd.Parameters.AddWithValue("@Stock", producto.Stock);

        var id = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(id);
    }

    public async Task ActualizarAsync(Producto producto)
    {
        const string sql = """
            UPDATE Productos
            SET Nombre = @Nombre,
                Codigo = @Codigo,
                Precio = @Precio,
                Stock  = @Stock
            WHERE Id = @Id
            """;

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", producto.Id);
        cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
        cmd.Parameters.AddWithValue("@Codigo", producto.Codigo);
        cmd.Parameters.AddWithValue("@Precio", producto.Precio);
        cmd.Parameters.AddWithValue("@Stock", producto.Stock);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DesactivarAsync(int id)
    {
        const string sql = "UPDATE Productos SET Estado = 0 WHERE Id = @Id";

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task<List<Producto>> EjecutarListaAsync(string sql, MySqlCommand? cmdExterno)
    {
        var resultado = new List<Producto>();

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            resultado.Add(MapearProducto(reader));

        return resultado;
    }

    private static Producto MapearProducto(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("Id"),
        Nombre = reader.GetString("Nombre"),
        Codigo = reader.GetString("Codigo"),
        Precio = reader.GetDecimal("Precio"),
        Stock = reader.GetInt32("Stock"),
        Estado = reader.GetBoolean("Estado")
    };
}
