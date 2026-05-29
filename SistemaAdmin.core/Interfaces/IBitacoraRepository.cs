using SistemaAdmin.core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.core.Interfaces;

public interface IBitacoraRepository
{
    Task RegistrarAsync(string nivel, string mensaje, string? stackTrace = null);
    Task<IEnumerable<BitacoraError>> ObtenerRecientesAsync(int cantidad = 50);
}
