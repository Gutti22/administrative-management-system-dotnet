using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAdmin.Application.Services;

using SistemaAdmin.core.DTOs;
using SistemaAdmin.core.Interfaces;
using SistemaAdmin.Core.Interfaces;

public class ReporteService
{
    private readonly IReporteRepository _reporteRepo;
    private readonly IBitacoraRepository _bitacora;

    public ReporteService(IReporteRepository reporteRepo, IBitacoraRepository bitacora)
    {
        _reporteRepo = reporteRepo;
        _bitacora = bitacora;
    }

    public async Task<IEnumerable<ProductoMasVendidoDto>> TopProductosAsync()
    {
        try
        {
            return await _reporteRepo.TopProductosVendidosAsync();
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("ERROR", "Error al generar reporte de productos", ex.ToString());
            throw;
        }
    }

    public async Task<IEnumerable<ClienteTopDto>> ClientesTopAsync()
    {
        try
        {
            return await _reporteRepo.ClientesConMayorFacturacionAsync();
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("ERROR", "Error al generar reporte de clientes", ex.ToString());
            throw;
        }
    }

    public async Task<IEnumerable<ProductoStockBajoDto>> StockBajoAsync()
    {
        try
        {
            return await _reporteRepo.ProductosConStockBajoAsync();
        }
        catch (Exception ex)
        {
            await _bitacora.RegistrarAsync("ERROR", "Error al generar reporte de stock", ex.ToString());
            throw;
        }
    }
}
