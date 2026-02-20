using AutoMapper;
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
    private readonly IMapper mapper;

    public TransaccionesController(IServicioUsuarios servicioUsuarios, IReposotorioCuentas reposotorioCuentas, IRepositorioCategorias repositorioCategorias, IRepositorioTransacciones repositorioTransacciones, IMapper mapper)
    {
        this.servicioUsuarios = servicioUsuarios;
        this.reposotorioCuentas = reposotorioCuentas;
        this.repositorioCategorias = repositorioCategorias;
        this.repositorioTransacciones = repositorioTransacciones;
        this.mapper = mapper;
    }

    public IActionResult Index()
    {
        return View();
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

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var usuarioId = servicioUsuarios.ObtenerUsuarioId();
        var transaccion = await repositorioTransacciones.ObtenerPorId(id, usuarioId);

        if (transaccion is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        var modelo = mapper.Map<TransaccionActualizacionViewModel>(transaccion);

        if(modelo.TipoOperacionId == TipoOperacion.Gasto)
        {
            modelo.MontoAnterior = modelo.Monto * -1;
        }

        modelo.CuentaAnteriorId = transaccion.CuentaId;
        modelo.Categorias = await ObtenerCategorias(usuarioId, transaccion.TipoOperacionId);
        modelo.Cuentas = await ObtenerCuentas(usuarioId);

        return View(modelo);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(TransaccionActualizacionViewModel model)
    {
        var usuarioId = servicioUsuarios.ObtenerUsuarioId();

        if (!ModelState.IsValid)
        {
            model.Cuentas = await ObtenerCuentas(usuarioId);
            model.Categorias = await ObtenerCategorias(usuarioId, model.TipoOperacionId);
            return View(model);
        }

        var cuenta = await reposotorioCuentas.ObtenerPorId(model.CuentaId, usuarioId);

        if (cuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        var categoria = await repositorioCategorias.ObtenerPorId(model.CategoriaId, usuarioId);

         if (categoria is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        var transaccion = mapper.Map<Transaccion>(model);

        model.MontoAnterior = model.Monto;

        if (model.TipoOperacionId == TipoOperacion.Gasto)
        {
            transaccion.Monto *= -1;
        }

        await repositorioTransacciones.Actualizar(transaccion, model.MontoAnterior, model.CuentaAnteriorId);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Borrar(int id)
    {
        var usuarioId = servicioUsuarios.ObtenerUsuarioId();

        var transaccion = await repositorioTransacciones.ObtenerPorId(id, usuarioId);

        if (transaccion is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        await repositorioTransacciones.Borrar(id);
        return RedirectToAction("Index");
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
