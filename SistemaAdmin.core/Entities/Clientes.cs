using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.core.Entities;

    public class Clientes
    {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Identidad {  get; set; }
    public string? Telefono { get; set; }  
    public string? Correo { get; set; }
    public DateTime FecRegistro { get; set; }
    public bool Estado {  get; set; } = true;
    }



