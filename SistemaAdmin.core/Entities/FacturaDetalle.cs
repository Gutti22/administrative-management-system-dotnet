using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.core.Entities;

public class FacturaDetalle
{
    public int Id { get; set; }
    public int FacturaId { get; set; }
    public int ProductoId { get; set; }
    public string ProductoName { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal SubTotal => Cantidad * PrecioUnitario;

}
