using SistemaAdmin.core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.core.Interfaces;

public interface IFacturaRepository
{
    Task<IEnumerable<Factura>> ObtenerTodosAsync();
    Task<Factura?> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(Factura factura);
    
}
