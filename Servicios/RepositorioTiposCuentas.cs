using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicios;

public interface IReposotorioTiposCuentas
{
    Task Crear(TipoCuenta tipoCuenta);

    Task<bool> Existe(string nombre, int usuarioId);
}

public class RepositorioTiposCuentas: IReposotorioTiposCuentas
{
    private readonly string connectionString;
    public RepositorioTiposCuentas(IConfiguration configuration)
    {
        this.connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task Crear(TipoCuenta tipoCuenta)
    {
        using var connection = new SqlConnection(connectionString);
        var id = await connection.QuerySingleAsync<int>($@"INSERT INTO TiposCuentas (Nombre, UsuarioId, Orden) VALUES (@Nombre, @UsuarioId, 0);
        SELECT SCOPE_IDENTITY();", tipoCuenta);

        tipoCuenta.Id = id;
    }

    public async Task<bool> Existe(string nombre, int usuarioId)
    {
        using var connection = new SqlConnection(connectionString);
        var existe = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1 from TiposCuentas where Nombre = @Nombre AND UsuarioId = @UsuarioId;", new {nombre, usuarioId});

        return existe == 1;
    }
    
}
