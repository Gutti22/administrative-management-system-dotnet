using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.core.Entities;

public class Factura
{
    public int Id { get; set; }
    public string NameFactura { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string ClienteName { get; set; } = string.Empty;
    public DateTime Fecha {  get; set; }
    public decimal Subtotal { get; set; }
    public decimal ISV { get; set; }
    public decimal Total { get; set; }
    public List<FacturaDetalle> Detalles { get; set; } = [];
}
