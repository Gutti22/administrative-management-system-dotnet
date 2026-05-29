using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.core.Entities;

public class BitacoraError
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Nivel { get; set; }
    public string Mensaje { get; set; }
    public string? StackTrace { get; set; }
}
