using System.Data;
using Microsoft.Data.SqlClient;

namespace TP_ControlVehicular.Tests.EndToEnd.Infra
{
    public static class SqlServerReachability
    {
        public static bool TryConnect(string connectionString, out Exception? exception)
        {
            exception = null;
            try
            {
                using var conn = new SqlConnection(connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteScalar();
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }
    }
}