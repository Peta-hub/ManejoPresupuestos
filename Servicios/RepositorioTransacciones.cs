using System.Transactions;
using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicios;

public interface IRepositorioTransacciones
{
    Task Crear(Transaccion transaction);
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
}
