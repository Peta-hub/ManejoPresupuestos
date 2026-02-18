using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuestos.Controllers;

public class TransaccionesController: Controller
{
    private readonly IServicioUsuarios servicioUsuarios;
    private readonly IReposotorioCuentas reposotorioCuentas;
    private readonly IRepositorioCategorias repositorioCategorias;
    private readonly IRepositorioTransacciones repositorioTransacciones;

    public TransaccionesController(IServicioUsuarios servicioUsuarios, IReposotorioCuentas reposotorioCuentas, IRepositorioCategorias repositorioCategorias, IRepositorioTransacciones repositorioTransacciones)
    {
        this.servicioUsuarios = servicioUsuarios;
        this.reposotorioCuentas = reposotorioCuentas;
        this.repositorioCategorias = repositorioCategorias;
        this.repositorioTransacciones = repositorioTransacciones;
    }

    public async Task<IActionResult> Crear()
    {
        var usuarioId = servicioUsuarios.ObtenerUsuarioId();
        var modelo = new TransaccionCreacionViewModel();
        modelo.Cuentas = await ObtenerCuentas(usuarioId);
        modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
        return View(modelo);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(TransaccionCreacionViewModel modelo)
    {
        var usuarioId = servicioUsuarios.ObtenerUsuarioId();

        if (!ModelState.IsValid)
        {
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
            return View(modelo);
        }

        var cuenta = await reposotorioCuentas.ObtenerPorId(modelo.CuentaId, usuarioId);

        if (cuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        var categoria = await repositorioCategorias.ObtenerPorId(modelo.CategoriaId, usuarioId);

        if (categoria is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        modelo.UsuarioId = usuarioId;

        if (modelo.TipoOperacionId == TipoOperacion.Gasto)
        {
            modelo.Monto *= -1;
        }

        await repositorioTransacciones.Crear(modelo);
        return RedirectToAction("");
    }

    private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId)
    {
        var cuentas = await reposotorioCuentas.Buscar(usuarioId);
        return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
    }

    private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId, TipoOperacion tipoOperacion)
    {
        var categorias = await repositorioCategorias.Obtener(usuarioId, tipoOperacion);
        return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
    }

    [HttpPost]
    public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
    {
        var usuarioId = servicioUsuarios.ObtenerUsuarioId();
        var categorias = await ObtenerCategorias(usuarioId, tipoOperacion);
        return Ok(categorias);

    }
}
