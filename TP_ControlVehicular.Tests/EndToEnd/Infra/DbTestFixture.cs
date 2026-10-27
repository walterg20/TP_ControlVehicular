using System;

namespace TP_ControlVehicular.Tests.EndToEnd.Infra
{
    public class DbTestFixture : IDisposable
    {
        public DbTestFixture()
        {
            TestRunContext.RunMarker = $"TDD-{DateTime.Now:yyyyMMddHHmmss}";
            TestRunContext.ConnectionString = SqlConnectionResolver.ResolveConnectionString();
            DbProvisioner.Provision();
        }

        public void Dispose()
        {
        }
    }
}