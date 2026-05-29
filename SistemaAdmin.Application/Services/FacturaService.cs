using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.Application.Services;

using SistemaAdmin.Application.DTOs;
using SistemaAdmin.core.Entities;
using SistemaAdmin.core.Interfaces;


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

        try
        {
            foreach (var item in dto.Detalles)
            {
                var producto = await _productoRepo.ObtenerPorIdAsync(item.ProductoId);

                if (producto is null)
                    return (false, $"El producto con Id {item.ProductoId} no existe.", 0);

                if (producto.Stock < item.Cantidad)
                    return (false, $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}.", 0);
            }

            var factura = ConstruirFactura(dto);

            var id = await _facturaRepo.CrearAsync(factura);
            return (true, "Factura creada correctamente.", id);
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("EXCEPTION", "Error al crear factura", ex.ToString());
            return (false, "Ocurrió un error al guardar la factura.", 0);
        }
    }

    private static Factura ConstruirFactura(FacturaCreateDto dto)
    {
        var detalles = dto.Detalles.Select(d => new FacturaDetalle
        {
            ProductoId = d.ProductoId,
            ProductoName = d.ProductoNombre,
            Cantidad = d.Cantidad,
            PrecioUnitario = d.PrecioUnitario
        }).ToList();

        var subtotal = detalles.Sum(d => d.SubTotal);
        var isv = Math.Round(subtotal * TasaISV, 2);
        var total = subtotal + isv;

        return new Factura
        {
            ClienteId = dto.ClienteId,
            Subtotal = subtotal,
            ISV = isv,
            Total = total,
            Detalles = detalles
        };
    }
}
