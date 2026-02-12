using AutoMapper;
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
    private readonly IMapper mapper;

    public CuentasController(IReposotorioTiposCuentas reposotorioTiposCuentas, IServicioUsuarios servicioUsuarios, IReposotorioCuentas reposotorioCuentas, IMapper mapper)
    {
        this.reposotorioTiposCuentas = reposotorioTiposCuentas;
        this.servicioUsuarios = servicioUsuarios;
        this.reposotorioCuentas = reposotorioCuentas;
        this.mapper = mapper;
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

    public async Task<IActionResult> Editar(int id)
    {
        var usuarioId = servicioUsuarios.ObtenerUsuarioId();
        var cuenta = await reposotorioCuentas.ObtenerPorId(id, usuarioId);

        if (cuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        var modelo = mapper.Map<CuentaCreacionViewModel>(cuenta);

        modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
        return View(modelo);
    }


    [HttpPost]
    public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
    {
        var usuarioId = servicioUsuarios.ObtenerUsuarioId();
        var cuenta = await reposotorioCuentas.ObtenerPorId(cuentaEditar.Id, usuarioId);

        if (cuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        var tipoCuenta = await reposotorioTiposCuentas.ObtenerPorId(cuentaEditar.TipoCuentaId, usuarioId);

        if (tipoCuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        await reposotorioCuentas.Actualizar(cuentaEditar);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Borrar(int id)
    {
        var usuarioId = servicioUsuarios.ObtenerUsuarioId();
        var cuenta = await reposotorioCuentas.ObtenerPorId(id, usuarioId);

        if (cuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        return View(cuenta);
    }

    [HttpPost]
    public async Task<IActionResult> BorrarCuenta(int id)
    {
        var usuarioId = servicioUsuarios.ObtenerUsuarioId();
        var cuenta = await reposotorioCuentas.ObtenerPorId(id, usuarioId);

        if (cuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        await reposotorioCuentas.Borrar(id);
        return RedirectToAction("Index");
    }

    public async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
    {
        var tiposCuentas = await reposotorioTiposCuentas.Obtener(usuarioId);
        return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
    }
}
