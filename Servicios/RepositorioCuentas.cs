using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.Data.SqlClient;
namespace ManejoPresupuestos.Servicios;

public interface IReposotorioCuentas
{
    Task Crear(Cuenta cuenta);

    Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
    Task<Cuenta> ObtenerPorId(int id, int usuarioId);

    Task Actualizar(CuentaCreacionViewModel cuenta);

    Task Borrar(int id);
}
public class RepositorioCuentas: IReposotorioCuentas
{
    private readonly string connectionString;
    public RepositorioCuentas(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task Crear(Cuenta cuenta)
    {
        using var connection = new SqlConnection(connectionString);
        var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Cuentas (Nombre, TipoCuentaId, Descripcion, Balance) VALUES (@Nombre, @TipoCuentaId, @Descripcion, @Balance); SELECT SCOPE_IDENTITY();", cuenta);
        cuenta.Id = id;
    }

    public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
    {
        using var connection = new SqlConnection(connectionString);
        return await connection.QueryAsync<Cuenta>(@"SELECT Cuentas.Id, Cuentas.Nombre, Balance, tc.Nombre AS TipoCuenta FROM Cuentas INNER JOIN TiposCuentas tc on tc.Id = Cuentas.TipoCuentaId where tc.UsuarioId = @UsuarioId order by tc.Orden",
        new {usuarioId});
    }

    public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
    {
        using var connection = new SqlConnection(connectionString);
        return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"SELECT Cuentas.Id, Cuentas.Nombre, Balance, Descripcion, Cuentas.TipoCuentaId FROM Cuentas INNER JOIN TiposCuentas tc on tc.Id = Cuentas.TipoCuentaId where tc.UsuarioId = @UsuarioId AND Cuentas.Id = @Id", new {id, usuarioId});
    }

    public async Task Actualizar(CuentaCreacionViewModel cuenta)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync(@"Update Cuentas SET Nombre = @Nombre, Balance = @Balance, Descripcion = @Descripcion, TipoCuentaId = @TipoCuentaId WHERE Id = @Id;", cuenta);
    }

    public async Task Borrar(int id)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync("DELETE Cuentas WHERE Id = @Id", new {id});
    }
}
