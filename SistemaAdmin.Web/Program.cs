using SistemaAdmin.Application.Services;
using SistemaAdmin.core.Interfaces;
using SistemaAdmin.Core.Interfaces;
using SistemaAdmin.Infrastructure.Repositories;
using SistemasAdmin.Infrastructure.Data;
using SistemasAdmin.Infrastructure.Logging;
using SistemasAdmin.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

//  Blazor 
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//  Base de datos 
var connectionString = builder.Configuration.GetConnectionString("MySql")
    ?? throw new InvalidOperationException("Connection string 'MySql' no encontrada.");

builder.Services.AddSingleton(new DbConnectionFactory(connectionString));

//  Repositorios
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
builder.Services.AddScoped<IReporteRepository, ReporteRepository>();
builder.Services.AddScoped<IBitacoraRepository, BitacoraRepository>();

//  Servicios de aplicación 
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<FacturaService>();
builder.Services.AddScoped<ReporteService>();
builder.Services.AddScoped<BitacoraService>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<SistemaAdmin.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();