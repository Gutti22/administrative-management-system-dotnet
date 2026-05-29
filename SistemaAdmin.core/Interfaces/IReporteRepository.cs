using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.Core.Interfaces;

using SistemaAdmin.core.DTOs;

public interface IReporteRepository
{
    Task<IEnumerable<ProductoMasVendidoDto>> TopProductosVendidosAsync();
    Task<IEnumerable<ClienteTopDto>> ClientesConMayorFacturacionAsync();
    Task<IEnumerable<ProductoStockBajoDto>> ProductosConStockBajoAsync();
}
