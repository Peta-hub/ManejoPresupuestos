namespace ManejoPresupuestos.Models;

public class TransaccionActualizacionViewModel: TransaccionCreacionViewModel
{
    public int CuentaAnterirorId { get; set; }
    public decimal MontoAnterior { get; set; }
}
