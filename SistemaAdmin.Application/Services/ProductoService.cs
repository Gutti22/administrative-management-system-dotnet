using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.Application.Services;

using SistemaAdmin.core.Entities;
using SistemaAdmin.core.Interfaces;


public class ProductoService
{
    private readonly IProductoRepository _productoRepo;
    private readonly IBitacoraRepository _bitacora;

    public ProductoService(IProductoRepository productoRepo, IBitacoraRepository bitacora)
    {
        _productoRepo = productoRepo;
        _bitacora = bitacora;
    }

    public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
    {
        try
        {
            return await _productoRepo.ObtenerTodosAsync();
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("ERROR", "Error al obtener productos", ex.ToString());
            throw;
        }
    }

    public async Task<IEnumerable<Producto>> ObtenerActivosAsync()
    {
        try
        {
            return await _productoRepo.ObtenerActivosAsync();
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("ERROR", "Error al obtener productos activos", ex.ToString());
            throw;
        }
    }

    public async Task<Producto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            return await _productoRepo.ObtenerPorIdAsync(id);
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("ERROR", $"Error al obtener producto Id={id}", ex.ToString());
            throw;
        }
    }

    public async Task<(bool exito, string mensaje)> CrearAsync(Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            return (false, "El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(producto.Codigo))
            return (false, "El código es obligatorio.");

        if (producto.Precio <= 0)
            return (false, "El precio debe ser mayor a cero.");

        if (producto.Stock < 0)
            return (false, "El stock no puede ser negativo.");

        try
        {
            await _productoRepo.CrearAsync(producto);
            return (true, "Producto creado correctamente.");
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("EXCEPTION", "Error al crear producto", ex.ToString());
            return (false, "Ocurrió un error al guardar el producto.");
        }
    }

    public async Task<(bool exito, string mensaje)> ActualizarAsync(Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            return (false, "El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(producto.Codigo))
            return (false, "El código es obligatorio.");

        if (producto.Precio <= 0)
            return (false, "El precio debe ser mayor a cero.");

        try
        {
            await _productoRepo.ActualizarAsync(producto);
            return (true, "Producto actualizado correctamente.");
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("EXCEPTION", "Error al actualizar producto", ex.ToString());
            return (false, "Ocurrió un error al actualizar el producto.");
        }
    }

    public async Task<(bool exito, string mensaje)> DesactivarAsync(int id)
    {
        try
        {
            await _productoRepo.DesactivarAsync(id);
            return (true, "Producto desactivado correctamente.");
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("EXCEPTION", "Error al desactivar producto", ex.ToString());
            return (false, "Ocurrió un error al desactivar el producto.");
        }
    }
}
