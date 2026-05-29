using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.core.DTOs;

public class ProductoMasVendidoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public int TotalUnidadesVendidas { get; set; }
    public decimal TotalFacturado { get; set; }
}

public class ClienteTopDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Identidad { get; set; } = string.Empty;
    public string? Correo { get; set; }
    public int TotalFacturas { get; set; }
    public decimal MontoTotal { get; set; }
}

public class ProductoStockBajoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public int Stock { get; set; }
    public decimal Precio { get; set; }
}
