using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.Application.DTOs;

public class FacturaCreateDto
{
    public int ClienteId { get; set; }
    public List<FacturaDetalleDto> Detalles { get; set; } = [];
}

public class FacturaDetalleDto
{
    public int ProductoId { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
