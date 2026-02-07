using Dapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace ManejoPresupuestos.Controllers
{
    public class TiposCuentasController: Controller
    {

        private readonly IReposotorioTiposCuentas reposotorioTiposCuentas;

        public TiposCuentasController(IReposotorioTiposCuentas reposotorioTiposCuentas)
        {
            this.reposotorioTiposCuentas = reposotorioTiposCuentas;
        }

        public IActionResult Crear()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
               return View(tipoCuenta); 
            }

            tipoCuenta.UsuarioId = 1;

            var yaExisteTipoCUenta = await reposotorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

            if (yaExisteTipoCUenta)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe.");

                return View(tipoCuenta);
            }

            await reposotorioTiposCuentas.Crear(tipoCuenta);

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var usuarioId = 1;
            var yaExisteTipoCUenta = await reposotorioTiposCuentas.Existe(nombre, usuarioId);

            if (yaExisteTipoCUenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);
        }

    }
}