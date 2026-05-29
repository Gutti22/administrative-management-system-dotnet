namespace SistemasAdmin.Infrastructure.Logging;

using MySqlConnector;
using SistemaAdmin.core.Entities;
using SistemaAdmin.core.Interfaces;
using SistemasAdmin.Infrastructure.Data;

public class BitacoraRepository : IBitacoraRepository
{
    private readonly DbConnectionFactory _factory;

    public BitacoraRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task RegistrarAsync(string nivel, string mensaje, string? stackTrace = null)
    {
        const string sql = "INSERT INTO BitacoraErrores (Nivel, Mensaje, StackTrace) VALUES (@Nivel, @Mensaje, @StackTrace)";

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Nivel", nivel);
        cmd.Parameters.AddWithValue("@Mensaje", mensaje);
        cmd.Parameters.AddWithValue("@StackTrace", (object?)stackTrace ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<BitacoraError>> ObtenerRecientesAsync(int cantidad = 50)
    {
        const string sql = "SELECT Id, Fecha, Nivel, Mensaje, StackTrace FROM BitacoraErrores ORDER BY Fecha DESC LIMIT @Cantidad";

        var resultado = new List<BitacoraError>();

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Cantidad", cantidad);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            resultado.Add(new BitacoraError
            {
                Id = reader.GetInt32(0),
                Fecha = reader.GetDateTime(1),
                Nivel = reader.GetString(2),
                Mensaje = reader.GetString(3),
                StackTrace = reader.IsDBNull(4) ? null : reader.GetString(4)
            });
        }

        return resultado;
    }
}