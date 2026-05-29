using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.Infrastructure.Repositories;

using MySqlConnector;
using SistemaAdmin.core.Entities;
using SistemaAdmin.core.Interfaces;
using SistemasAdmin.Infrastructure.Data;

public class ClienteRepository : IClienteRepository
{
    private readonly DbConnectionFactory _factory;

    public ClienteRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Clientes>> ObtenerTodosAsync()
    {
        const string sql = """
            SELECT Id, Nombre, Identidad, Telefono, Correo, FechaRegistro, Estado
            FROM Clientes
            ORDER BY Nombre
            """
        ;

        return await EjecutarListaAsync(sql);
    }

    public async Task<IEnumerable<Clientes>> BuscarAsync(string termino)
    {
        const string sql = """
            SELECT Id, Nombre, Identidad, Telefono, Correo, FechaRegistro, Estado
            FROM Clientes
            WHERE Nombre LIKE @Termino
               OR Identidad LIKE @Termino
               OR Correo LIKE @Termino
            ORDER BY Nombre
            """;

        var resultado = new List<Clientes>();

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Termino", $"%{termino}%");

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            resultado.Add(MapearCliente(reader));

        return resultado;
    }

    public async Task<Clientes?> ObtenerPorIdAsync(int id)
    {
        const string sql = """
            SELECT Id, Nombre, Identidad, Telefono, Correo, FechaRegistro, Estado
            FROM Clientes
            WHERE Id = @Id
            """;

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapearCliente(reader) : null;
    }

    public async Task<int> CrearAsync(Clientes cliente)
    {
        const string sql = """
            INSERT INTO Clientes (Nombre, Identidad, Telefono, Correo)
            VALUES (@Nombre, @Identidad, @Telefono, @Correo);
            SELECT LAST_INSERT_ID();
            """;

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
        cmd.Parameters.AddWithValue("@Identidad", cliente.Identidad);
        cmd.Parameters.AddWithValue("@Telefono", (object?)cliente.Telefono ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Correo", (object?)cliente.Correo ?? DBNull.Value);

        var id = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(id);
    }

    public async Task ActualizarAsync(Clientes cliente)
    {
        const string sql = """
            UPDATE Clientes
            SET Nombre       = @Nombre,
                Identidad = @Identidad,
                Telefono     = @Telefono,
                Correo       = @Correo
            WHERE Id = @Id
            """;

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", cliente.Id);
        cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
        cmd.Parameters.AddWithValue("@Identidad", cliente.Identidad);
        cmd.Parameters.AddWithValue("@Telefono", (object?)cliente.Telefono ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Correo", (object?)cliente.Correo ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task CambiarEstadoAsync(int id, bool estado)
    {
        const string sql = "UPDATE Clientes SET Estado = @Estado WHERE Id = @Id";

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Estado", estado);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task<List<Clientes>> EjecutarListaAsync(string sql)
    {
        var resultado = new List<Clientes>();

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            resultado.Add(MapearCliente(reader));

        return resultado;
    }
    private static Clientes MapearCliente(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("Id"),
        Nombre = reader.GetString("Nombre"),
        Identidad = reader.GetString("Identidad"),
        Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
        Correo = reader.IsDBNull(reader.GetOrdinal("Correo")) ? null : reader.GetString("Correo"),
        FecRegistro = reader.GetDateTime("FechaRegistro"),
        Estado = reader.GetBoolean("Estado")
    };
}
