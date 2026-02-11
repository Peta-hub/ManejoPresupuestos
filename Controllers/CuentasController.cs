using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace ManejoPresupuestos.Controllers;

public class CuentasController: Controller
{
    private readonly IReposotorioTiposCuentas reposotorioTiposCuentas;
    private readonly IServicioUsuarios servicioUsuarios;
    private readonly IReposotorioCuentas reposotorioCuentas;

    public CuentasController(IReposotorioTiposCuentas reposotorioTiposCuentas, IServicioUsuarios servicioUsuarios, IReposotorioCuentas reposotorioCuentas)
    {
        this.reposotorioTiposCuentas = reposotorioTiposCuentas;
        this.servicioUsuarios = servicioUsuarios;
        this.reposotorioCuentas = reposotorioCuentas;
    }

    public async Task<IActionResult> Index()
    {
        var usuarioId = servicioUsuarios.ObtenerUsuarioId();
        var cuentasConTipoCuenta = await reposotorioCuentas.Buscar(usuarioId);

        var modelo = cuentasConTipoCuenta.GroupBy(x => x.TipoCuenta).Select(grupo => new IndiceCuentasViewModel
        {
            TipoCuenta = grupo.Key,
            Cuentas = grupo.AsEnumerable()
        }).ToList();

        return View(modelo);
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var usuarioId = servicioUsuarios.ObtenerUsuarioId();
        var modelo = new CuentaCreacionViewModel();
        var tiposCuentas = await reposotorioTiposCuentas.Obtener(usuarioId);
        modelo.TiposCuentas = tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));

        return View(modelo);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
    {
        var usuarioId = servicioUsuarios.ObtenerUsuarioId();
        var tipoCuenta = await reposotorioTiposCuentas.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);

        if (tipoCuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        if (!ModelState.IsValid)
        {
            cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View(cuenta);
        }

        await reposotorioCuentas.Crear(cuenta);
        return RedirectToAction("Index");
    }

    public async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
    {
        var tiposCuentas = await reposotorioTiposCuentas.Obtener(usuarioId);
        return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
    }
}
