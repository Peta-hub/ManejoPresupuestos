using System.ComponentModel.DataAnnotations;
using ManejoPresupuestos.Validaciones;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuestos.Models
{
    public class TipoCuenta //: IValidatableObject, esta es otra forma de validar el formulario, hace lo mismo que la validacion [PrimeraLetraMayuscula] solo que con la funcion que esta comentada abajo
    {
        public int Id {get; set;}
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [PrimeraLetraMayuscula]
        [Remote(action: "VerificarExisteTipoCuenta", controller: "TiposCuentas")]
        public string Nombre { get; set; }

        public int UsuarioId {get; set;}
        public int Orden {get; set;}

        // public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        // {
        //     if (Nombre != null)
        //     {
        //         var primeraLetra = Nombre[0].ToString();

        //         if (primeraLetra != primeraLetra.ToUpper())
        //         {
        //             yield return new ValidationResult("La primera letra debe ser mayuscula", new[] {nameof(Nombre)});
        //         }
        //     }
        // }
    }
}