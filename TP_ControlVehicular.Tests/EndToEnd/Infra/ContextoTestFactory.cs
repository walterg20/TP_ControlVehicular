using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;

namespace TP_ControlVehicular.Tests.EndToEnd.Infra
{
    public static class ContextoTestFactory
    {
        public static CVDbContext Create()
        {
            var options = new DbContextOptionsBuilder<CVDbContext>()
                .UseSqlServer(TestRunContext.ConnectionString)
                .Options;
            return new CVDbContext(options);
        }
    }
}