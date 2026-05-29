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

public class FacturaRepository : IFacturaRepository
{
    private readonly DbConnectionFactory _factory;

    public FacturaRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Factura>> ObtenerTodosAsync()
    {
        const string sql = """
            SELECT f.Id, f.NumeroFactura, f.ClienteId, c.Nombre AS ClienteNombre,
                   f.Fecha, f.Subtotal, f.ISV, f.Total
            FROM Facturas f
            INNER JOIN Clientes c ON c.Id = f.ClienteId
            ORDER BY f.Fecha DESC
            """;

        var resultado = new List<Factura>();

        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            resultado.Add(MapearFacturaCabecera(reader));

        return resultado;
    }

    public async Task<Factura?> ObtenerPorIdAsync(int id)
    {
        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();

        var factura = await ObtenerCabeceraAsync(conn, id);
        if (factura is null) return null;

        factura.Detalles = await ObtenerDetallesAsync(conn, id);
        return factura;
    }

    public async Task<int> CrearAsync(Factura factura)
    {
        await using var conn = _factory.CreateConection();
        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            factura.NameFactura = await GenerarNumeroFacturaAsync(conn, transaction);

            var facturaId = await InsertarCabeceraAsync(conn, transaction, factura);

            foreach (var detalle in factura.Detalles)
            {
                await InsertarDetalleAsync(conn, transaction, facturaId, detalle);
                await DescontarStockAsync(conn, transaction, detalle.ProductoId, detalle.Cantidad);
            }

            await transaction.CommitAsync();
            return facturaId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<string> GenerarNumeroFacturaAsync(MySqlConnection conn, MySqlTransaction tx)
    {
        const string sql = "SELECT COALESCE(MAX(Id), 0) + 1 FROM Facturas";

        await using var cmd = new MySqlCommand(sql, conn, tx);
        var siguiente = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return $"FACT-{siguiente:D6}";
    }

    private async Task<int> InsertarCabeceraAsync(MySqlConnection conn, MySqlTransaction tx, Factura factura)
    {
        const string sql = """
            INSERT INTO Facturas (NumeroFactura, ClienteId, Subtotal, ISV, Total)
            VALUES (@NumeroFactura, @ClienteId, @Subtotal, @ISV, @Total);
            SELECT LAST_INSERT_ID();
            """;

        await using var cmd = new MySqlCommand(sql, conn, tx);
        cmd.Parameters.AddWithValue("@NumeroFactura", factura.NameFactura);
        cmd.Parameters.AddWithValue("@ClienteId", factura.ClienteId);
        cmd.Parameters.AddWithValue("@Subtotal", factura.Subtotal);
        cmd.Parameters.AddWithValue("@ISV", factura.ISV);
        cmd.Parameters.AddWithValue("@Total", factura.Total);

        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    private async Task InsertarDetalleAsync(MySqlConnection conn, MySqlTransaction tx, int facturaId, FacturaDetalle detalle)
    {
        const string sql = """
            INSERT INTO FacturaDetalles (FacturaId, ProductoId, Cantidad, PrecioUnitario, Subtotal)
            VALUES (@FacturaId, @ProductoId, @Cantidad, @PrecioUnitario, @Subtotal)
            """;

        await using var cmd = new MySqlCommand(sql, conn, tx);
        cmd.Parameters.AddWithValue("@FacturaId", facturaId);
        cmd.Parameters.AddWithValue("@ProductoId", detalle.ProductoId);
        cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
        cmd.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);
        cmd.Parameters.AddWithValue("@Subtotal", detalle.SubTotal);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task DescontarStockAsync(MySqlConnection conn, MySqlTransaction tx, int productoId, int cantidad)
    {
        const string sql = "UPDATE Productos SET Stock = Stock - @Cantidad WHERE Id = @ProductoId";

        await using var cmd = new MySqlCommand(sql, conn, tx);
        cmd.Parameters.AddWithValue("@ProductoId", productoId);
        cmd.Parameters.AddWithValue("@Cantidad", cantidad);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task<Factura?> ObtenerCabeceraAsync(MySqlConnection conn, int id)
    {
        const string sql = """
            SELECT f.Id, f.NumeroFactura, f.ClienteId, c.Nombre AS ClienteNombre,
                   f.Fecha, f.Subtotal, f.ISV, f.Total
            FROM Facturas f
            INNER JOIN Clientes c ON c.Id = f.ClienteId
            WHERE f.Id = @Id
            """;

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapearFacturaCabecera(reader) : null;
    }

    private async Task<List<FacturaDetalle>> ObtenerDetallesAsync(MySqlConnection conn, int facturaId)
    {
        const string sql = """
            SELECT fd.Id, fd.FacturaId, fd.ProductoId, p.Nombre AS ProductoNombre,
                   fd.Cantidad, fd.PrecioUnitario, fd.Subtotal
            FROM FacturaDetalles fd
            INNER JOIN Productos p ON p.Id = fd.ProductoId
            WHERE fd.FacturaId = @FacturaId
            """;

        var detalles = new List<FacturaDetalle>();

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@FacturaId", facturaId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            detalles.Add(new FacturaDetalle
            {
                Id = reader.GetInt32("Id"),
                FacturaId = reader.GetInt32("FacturaId"),
                ProductoId = reader.GetInt32("ProductoId"),
                ProductoName = reader.GetString("ProductoNombre"),
                Cantidad = reader.GetInt32("Cantidad"),
                PrecioUnitario = reader.GetDecimal("PrecioUnitario")
            });
        }

        return detalles;
    }

    private static Factura MapearFacturaCabecera(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("Id"),
        NameFactura = reader.GetString("NumeroFactura"),
        ClienteId = reader.GetInt32("ClienteId"),
        ClienteName = reader.GetString("ClienteNombre"),
        Fecha = reader.GetDateTime("Fecha"),
        Subtotal = reader.GetDecimal("Subtotal"),
        ISV = reader.GetDecimal("ISV"),
        Total = reader.GetDecimal("Total")
    };
}
