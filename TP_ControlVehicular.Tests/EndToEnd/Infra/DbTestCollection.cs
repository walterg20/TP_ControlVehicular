using Xunit;

namespace TP_ControlVehicular.Tests.EndToEnd.Infra
{
    [CollectionDefinition("e2e-sql")]
    public class DbTestCollection : ICollectionFixture<DbTestFixture>
    {
    }
}