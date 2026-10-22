using SistemaAdmin.Application.DTOs;
using SistemaAdmin.core.Entities;
using SistemaAdmin.core.Interfaces;

namespace SistemaAdmin.Application.Services;

public class FacturaService
{
    private const decimal TasaISV = 0.15m;

    private readonly IFacturaRepository _facturaRepo;
    private readonly IProductoRepository _productoRepo;
    private readonly IBitacoraRepository _bitacora;

    public FacturaService(
        IFacturaRepository facturaRepo,
        IProductoRepository productoRepo,
        IBitacoraRepository bitacora)
    {
        _facturaRepo = facturaRepo;
        _productoRepo = productoRepo;
        _bitacora = bitacora;
    }

    public async Task<IEnumerable<Factura>> ObtenerTodosAsync()
    {
        try
        {
            return await _facturaRepo.ObtenerTodosAsync();
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("ERROR", "Error al obtener facturas", ex.ToString());
            throw;
        }
    }

    public async Task<Factura?> ObtenerPorIdAsync(int id)
    {
        try
        {
            return await _facturaRepo.ObtenerPorIdAsync(id);
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("ERROR", $"Error al obtener factura Id={id}", ex.ToString());
            throw;
        }
    }

    public async Task<(bool exito, string mensaje, int facturaId)> CrearAsync(FacturaCreateDto dto)
    {
        if (dto.ClienteId <= 0)
            return (false, "Debe seleccionar un cliente.", 0);

        if (dto.Detalles.Count == 0)
            return (false, "La factura debe tener al menos un producto.", 0);

        if (dto.Detalles.Any(d => d.ProductoId <= 0 || d.Cantidad <= 0))
            return (false, "Todos los productos deben tener una cantidad válida mayor a cero.", 0);

        try
        {
            var detallesNormalizados = new List<FacturaDetalle>();

            foreach (var grupo in dto.Detalles.GroupBy(d => d.ProductoId))
            {
                var cantidadTotal = grupo.Sum(d => d.Cantidad);
                var producto = await _productoRepo.ObtenerPorIdAsync(grupo.Key);

                if (producto is null || !producto.Estado)
                    return (false, $"El producto con Id {grupo.Key} no está disponible.", 0);

                if (producto.Stock < cantidadTotal)
                    return (false,
                        $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}.",
                        0);

                detallesNormalizados.Add(new FacturaDetalle
                {
                    ProductoId = producto.Id,
                    ProductoName = producto.Nombre,
                    Cantidad = cantidadTotal,
                    PrecioUnitario = producto.Precio
                });
            }

            var factura = ConstruirFactura(dto.ClienteId, detallesNormalizados);
            var id = await _facturaRepo.CrearAsync(factura);

            return (true, "Factura creada correctamente.", id);
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("EXCEPTION", "Error al crear factura", ex.ToString());
            return (false,
                "No fue posible emitir la factura. Verifique el stock disponible e intente nuevamente.",
                0);
        }
    }

    private static Factura ConstruirFactura(int clienteId, List<FacturaDetalle> detalles)
    {
        var subtotal = detalles.Sum(d => d.SubTotal);
        var isv = Math.Round(subtotal * TasaISV, 2);
        var total = subtotal + isv;

        return new Factura
        {
            ClienteId = clienteId,
            Subtotal = subtotal,
            ISV = isv,
            Total = total,
            Detalles = detalles
        };
    }
}
