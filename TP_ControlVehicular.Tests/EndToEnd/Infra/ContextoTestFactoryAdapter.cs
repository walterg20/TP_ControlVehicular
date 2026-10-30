// walterg20
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;

namespace TP_ControlVehicular.Tests.EndToEnd.Infra
{
    /// <summary>
    /// Factory de contextos para las pruebas end-to-end. replica el comportamiento de
    /// <c>AddDbContextFactory</c> de la aplicacion: cada operacion de repositorio obtiene
    /// su propia instancia de contexto.
    /// </summary>
    public sealed class ContextoTestFactoryAdapter : IDbContextFactory<CVDbContext>
    {
        private readonly string _connectionString;

        public ContextoTestFactoryAdapter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public CVDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<CVDbContext>()
                .UseSqlServer(_connectionString)
                .Options;
            return new CVDbContext(options);
        }
    }
}
