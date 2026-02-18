using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuestos.Controllers;

public class TransaccionesController: Controller
{
    private readonly IServicioUsuarios servicioUsuarios;
    private readonly IReposotorioCuentas reposotorioCuentas;

    public TransaccionesController(IServicioUsuarios servicioUsuarios, IReposotorioCuentas reposotorioCuentas)
    {
        this.servicioUsuarios = servicioUsuarios;
        this.reposotorioCuentas = reposotorioCuentas;
    }

    public async Task<IActionResult> Crear()
    {
        var usuarioId = servicioUsuarios.ObtenerUsuarioId();
        var modelo = new TransaccionCreacionViewModel();
        modelo.Cuentas = await ObtenerCuentas(usuarioId);
        return View(modelo);
    }

    private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId)
    {
        var cuentas = await reposotorioCuentas.Buscar(usuarioId);
        return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
    }
}
