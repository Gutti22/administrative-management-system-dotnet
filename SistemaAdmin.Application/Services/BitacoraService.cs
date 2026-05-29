using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.Application.Services;

using SistemaAdmin.core.Entities;
using SistemaAdmin.core.Interfaces;

public class BitacoraService
{
    private readonly IBitacoraRepository _bitacora;

    public BitacoraService(IBitacoraRepository bitacora)
    {
        _bitacora = bitacora;
    }

    public async Task<IEnumerable<BitacoraError>> ObtenerRecientesAsync()
    {
        return await _bitacora.ObtenerRecientesAsync();
    }
}