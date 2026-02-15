using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicios;

public interface IRepositorioCategorias
{
    Task Crear(Categoria categoria);
}

public class RepositorioCategorias : IRepositorioCategorias
{
    private readonly string connectionString;
    public RepositorioCategorias(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task Crear(Categoria categoria)
    {
        using var connection = new SqlConnection(connectionString);
        var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Categorias (Nombre, TipoOperacionId, UsuarioId) Values (@Nombre, @TipoOperacionId, @UsuarioId); SELECT SCOPE_IDENTITY();", categoria);

        categoria.Id = id;
    }
}
