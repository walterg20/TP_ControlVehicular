using System;
using System.IO;
using Microsoft.Data.SqlClient;

namespace TP_ControlVehicular.Tests.EndToEnd.Infra
{
    public static class DbProvisioner
    {
        public static void Provision()
        {
            var connStr = TestRunContext.ConnectionString;
            var builder = new SqlConnectionStringBuilder(connStr);
            var dbName = builder.InitialCatalog;
            builder.InitialCatalog = "master";

            var masterConn = builder.ToString();
            using (var conn = new SqlConnection(masterConn))
            {
                conn.Open();
                var cmdCheck = new SqlCommand($@"
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{dbName}')
BEGIN
    CREATE DATABASE [{dbName}];
END", conn);
                cmdCheck.ExecuteNonQuery();
            }

            string[] scripts = new[]
            {
                "01_estructura.sql",
                "02_procedimientos.sql",
                "03_triggers.sql",
                "04_datos.sql"
            };

            foreach (var script in scripts)
            {
                var scriptPath = FindScript(script);
                if (scriptPath == null)
                {
                    continue;
                }
                var content = File.ReadAllText(scriptPath);
                content = content.Replace("USE [ControlVehicular]", $"USE [{dbName}]", StringComparison.OrdinalIgnoreCase);
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    // Split by GO (simple approach)
                    var parts = content.Split(new[] { "GO\r\n", "GO\n", "GO" }, StringSplitOptions.None);
                    foreach (var part in parts)
                    {
                        if (string.IsNullOrWhiteSpace(part))
                        {
                            continue;
                        }
                        var cmd = new SqlCommand(part, conn);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static string? FindScript(string name)
        {
            var basePath = AppContext.BaseDirectory;
            var candidate = Path.Combine(basePath, "bd", name);
            if (File.Exists(candidate))
            {
                return candidate;
            }
            var current = new DirectoryInfo(basePath);
            while (current != null)
            {
                candidate = Path.Combine(current.FullName, "bd", name);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
                candidate = Path.Combine(current.FullName, "TP_ControlVehicular.Tests", "bd", name);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
                current = current.Parent;
            }
            return null;
        }
    }
}