using System;

namespace TP_ControlVehicular.Tests.EndToEnd.Infra
{
    public static class TestRunContext
    {
        public static string RunMarker { get; set; } = $"TDD-{DateTime.Now:yyyyMMddHHmmss}";
        public static string ConnectionString { get; set; } = SqlConnectionResolver.ResolveConnectionString();
    }
}