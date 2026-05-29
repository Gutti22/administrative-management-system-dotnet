using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.core.Interfaces;

using SistemaAdmin.core.Entities;

public interface IClienteRepository
{
    Task<IEnumerable<Clientes>> ObtenerTodosAsync();
    Task<IEnumerable<Clientes>> BuscarAsync(string termino);
    Task<Clientes?> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(Clientes cliente);
    Task ActualizarAsync(Clientes cliente);
    Task CambiarEstadoAsync(int id, bool estado);
}
