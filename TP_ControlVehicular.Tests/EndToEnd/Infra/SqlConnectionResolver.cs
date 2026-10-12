using System;
using System.IO;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace TP_ControlVehicular.Tests.EndToEnd.Infra
{
    public static class SqlConnectionResolver
    {
        public static string ResolveConnectionString()
        {
            var envConn = Environment.GetEnvironmentVariable("TPCV_TEST_CONNECTION");
            if (!string.IsNullOrWhiteSpace(envConn))
            {
                return envConn;
            }

            var basePath = AppContext.BaseDirectory;
            var appSettingsPath = Path.Combine(basePath, "appsettings.json");
            if (!File.Exists(appSettingsPath))
            {
                // Try to find appsettings.json up the tree
                var current = new DirectoryInfo(basePath);
                while (current != null)
                {
                    var candidate = Path.Combine(current.FullName, "appsettings.json");
                    if (File.Exists(candidate))
                    {
                        appSettingsPath = candidate;
                        break;
                    }
                    current = current.Parent;
                }
            }

            if (File.Exists(appSettingsPath))
            {
                var json = File.ReadAllText(appSettingsPath);
                var jObject = JObject.Parse(json);
                var conn = jObject["ConnectionStrings"]?["DefaultConnection"]?.ToString();
                if (!string.IsNullOrWhiteSpace(conn))
                {
                    var builder = new SqlConnectionStringBuilder(conn);
                    builder.InitialCatalog = "ControlVehicular_Test";
                    return builder.ToString();
                }
            }

            // Fallback to a reasonable default
            var fallbackBuilder = new SqlConnectionStringBuilder
            {
                DataSource = @"(local)\SQLEXPRESS",
                InitialCatalog = "ControlVehicular_Test",
                IntegratedSecurity = true,
                TrustServerCertificate = true
            };
            return fallbackBuilder.ToString();
        }
    }
}