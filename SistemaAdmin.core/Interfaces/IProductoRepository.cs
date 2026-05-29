using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.core.Interfaces;

using SistemaAdmin.core.Entities;
public interface IProductoRepository
{
    Task<IEnumerable<Producto>> ObtenerTodosAsync();
    Task<IEnumerable<Producto>> ObtenerActivosAsync();
    Task<Producto?> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(Producto producto);
    Task ActualizarAsync(Producto producto);
    Task DesactivarAsync(int id);
}
