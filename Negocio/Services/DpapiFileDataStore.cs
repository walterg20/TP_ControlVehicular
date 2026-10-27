// walterg20
using Google.Apis.Util.Store;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TP_ControlVehicular.Negocio.Services
{
    /// <summary>
    /// Almacen de tokens de Google por-PC. Cada entrada se serializa a JSON y se
    /// cifra con DPAPI (ambito del usuario actual), de modo que el token de
    /// refresco nunca queda en texto plano ni viaja a la base de datos.
    /// </summary>
    internal sealed class DpapiFileDataStore : IDataStore
    {
        private static readonly byte[] Entropia = Encoding.UTF8.GetBytes("TP_ControlVehicular.Drive");

        private readonly string _carpeta;

        public DpapiFileDataStore()
        {
            _carpeta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "TP_ControlVehicular",
                "Drive");

            Directory.CreateDirectory(_carpeta);
        }

        public Task StoreAsync<T>(string key, T value)
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(value);
            var cifrado = ProtectedData.Protect(json, Entropia, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(RutaDe(key), cifrado);
            return Task.CompletedTask;
        }

        public Task DeleteAsync<T>(string key)
        {
            var ruta = RutaDe(key);

            if (File.Exists(ruta))
                File.Delete(ruta);

            return Task.CompletedTask;
        }

        public Task<T> GetAsync<T>(string key)
        {
            var ruta = RutaDe(key);

            if (!File.Exists(ruta))
                return Task.FromResult<T>(default!);

            try
            {
                var cifrado = File.ReadAllBytes(ruta);
                var json = ProtectedData.Unprotect(cifrado, Entropia, DataProtectionScope.CurrentUser);
                return Task.FromResult(JsonSerializer.Deserialize<T>(json)!);
            }
            catch
            {
                // Token corrupto, de otro usuario o de otro equipo: se descarta.
                return Task.FromResult<T>(default!);
            }
        }

        public Task ClearAsync()
        {
            if (Directory.Exists(_carpeta))
                Directory.Delete(_carpeta, true);

            return Task.CompletedTask;
        }

        private string RutaDe(string key)
        {
            var seguro = Convert.ToBase64String(Encoding.UTF8.GetBytes(key))
                .Replace('/', '_')
                .Replace('+', '-');

            return Path.Combine(_carpeta, seguro + ".dat");
        }
    }
}
