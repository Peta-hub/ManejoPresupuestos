using System.ComponentModel.DataAnnotations;
using ManejoPresupuestos.Validaciones;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace ManejoPresupuestos.Models;

public class Cuenta
{
    public int Id {get; set;}
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(maximumLength: 50)]
    [PrimeraLetraMayuscula]
    public string Nombre {get; set;}
    [Display(Name = "Tipo Cuenta")]
    public int TipoCuentaId {get; set;}
    public decimal Balance {get; set;}
    [StringLength(maximumLength: 1000)]
    public int Descripcion {get; set;}
}
