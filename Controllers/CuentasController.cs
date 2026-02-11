using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace ManejoPresupuestos.Controllers;

public class CuentasController: Controller
{
    private readonly IReposotorioTiposCuentas reposotorioTiposCuentas;
    private readonly IServicioUsuarios servicioUsuarios;

    public CuentasController(IReposotorioTiposCuentas reposotorioTiposCuentas, IServicioUsuarios servicioUsuarios)
    {
        this.reposotorioTiposCuentas = reposotorioTiposCuentas;
        this.servicioUsuarios = servicioUsuarios;
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var usuarioId = servicioUsuarios.ObtenerUsuarioId();
        var tiposCuentas = await reposotorioTiposCuentas.Obtener(usuarioId);
        var modelo = new CuentaCreacionViewModel();
        modelo.TiposCuentas = tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));

        return View(modelo);
    }
}
