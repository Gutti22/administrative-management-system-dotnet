using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.Application.Services;

using SistemaAdmin.core.Entities;
using SistemaAdmin.core.Interfaces;


public class ClienteService
{
    private readonly IClienteRepository _clienteRepo;
    private readonly IBitacoraRepository _bitacora;

    public ClienteService(IClienteRepository clienteRepo, IBitacoraRepository bitacora)
    {
        _clienteRepo = clienteRepo;
        _bitacora = bitacora;
    }

    public async Task<IEnumerable<Clientes>> ObtenerTodosAsync()
    {
        try
        {
            return await _clienteRepo.ObtenerTodosAsync();
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("ERROR", "Error al obtener clientes", ex.ToString());
            throw;
        }
    }

    public async Task<IEnumerable<Clientes>> BuscarAsync(string termino)
    {
        if (string.IsNullOrWhiteSpace(termino))
            return await ObtenerTodosAsync();

        try
        {
            return await _clienteRepo.BuscarAsync(termino);
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("ERROR", $"Error al buscar clientes: {termino}", ex.ToString());
            throw;
        }
    }

    public async Task<Clientes?> ObtenerPorIdAsync(int id)
    {
        try
        {
            return await _clienteRepo.ObtenerPorIdAsync(id);
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("ERROR", $"Error al obtener cliente Id={id}", ex.ToString());
            throw;
        }
    }

    public async Task<(bool exito, string mensaje)> CrearAsync(Clientes cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Nombre))
            return (false, "El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(cliente.Identidad))
            return (false, "La identidad es obligatoria.");

        try
        {
            cliente.Id = await _clienteRepo.CrearAsync(cliente);
            return (true, "Cliente creado correctamente.");
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("EXCEPTION", "Error al crear cliente", ex.ToString());
            return (false, "Ocurrió un error al guardar el cliente.");
        }
    }

    public async Task<(bool exito, string mensaje)> ActualizarAsync(Clientes cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Nombre))
            return (false, "El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(cliente.Identidad))
            return (false, "La identidad o RTN es obligatoria.");

        try
        {
            await _clienteRepo.ActualizarAsync(cliente);
            return (true, "Cliente actualizado correctamente.");
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("EXCEPTION", "Error al actualizar cliente", ex.ToString());
            return (false, "Ocurrió un error al actualizar el cliente.");
        }
    }

    public async Task<(bool exito, string mensaje)> CambiarEstadoAsync(int id, bool estado)
    {
        try
        {
            await _clienteRepo.CambiarEstadoAsync(id, estado);
            var accion = estado ? "activado" : "desactivado";
            return (true, $"Cliente {accion} correctamente.");
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("EXCEPTION", "Error al cambiar estado del cliente", ex.ToString());
            return (false, "Ocurrió un error al cambiar el estado.");
        }
    }
}
