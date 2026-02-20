using System.Transactions;
using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicios;

public interface IRepositorioTransacciones
{
    Task Crear(Transaccion transaction);
    Task Actualizar(Transaccion transaccion, decimal montoAnteriror, int cuentaAnteriror);
    Task<Transaccion> ObtenerPorId(int id, int usuarioId);
    Task Borrar(int id);
}

public class RepositorioTransacciones: IRepositorioTransacciones
{

    private readonly string connectionString;
    public RepositorioTransacciones(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task Crear(Transaccion transaction)
    {
        using var connection = new SqlConnection(connectionString);
        var id = await connection.QuerySingleAsync<int>("Transacciones_Insertar", new {transaction.UsuarioId, transaction.FechaTransaccion, transaction.Monto, transaction.CategoriaId, transaction.CuentaId, transaction.Nota}, 
        commandType: System.Data.CommandType.StoredProcedure);

        transaction.Id = id;
    }

    public async Task Actualizar(Transaccion transaccion, decimal montoAnteriror, int cuentaAnterirorId)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync("Transacciones_Actualizar", new {transaccion.Id, transaccion.FechaTransaccion, transaccion.Monto, transaccion.CategoriaId, transaccion.CuentaId, transaccion.Nota, montoAnteriror, cuentaAnterirorId},
         commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
    {
        using var connection = new SqlConnection(connectionString);
        return await connection.QueryFirstOrDefaultAsync<Transaccion>(@"select Transacciones.*, cat.TipoOperacionId from Transacciones INNER JOIN Categorias cat on cat.Id = Transacciones.CategoriaId where Transacciones.Id = @Id and Transacciones.UsuarioId = @UsuarioId",
         new {id, usuarioId});
    }

    public async Task Borrar(int id)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync("Transacciones_Borrar", new {id}, commandType: System.Data.CommandType.StoredProcedure);
    }
}
