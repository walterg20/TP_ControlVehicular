using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.Services;
using Xunit;

namespace TP_ControlVehicular.Tests.Services
{
    public class AyudaServiceTests
    {
        private static string CrearJsonTemp()
        {
            var json = @"{
  ""roles"": [
    {
      ""idRol"": 1,
      ""nombreRol"": ""Administrador"",
      ""ayudas"": [
        {
          ""idAyuda"": 1,
          ""nombrePantalla"": ""OrdenServicio"",
          ""nombreControl"": ""cmbVehiculos"",
          ""secciones"": [
            { ""idSeccion"": 1, ""titulo"": ""T1"", ""contenido"": ""C1"", ""orden"": 1, ""expandida"": true }
          ]
        }
      ]
    }
  ]
}";
            var path = Path.Combine(Path.GetTempPath(), $"ayudas_{Guid.NewGuid()}.json");
            File.WriteAllText(path, json);
            return path;
        }

        [Fact]
        public async Task ObtenerAyudaAsync_RolYControlExistente_DevuelveAyuda()
        {
            var path = CrearJsonTemp();
            try
            {
                var svc = new AyudaService(path);
                var ayuda = await svc.ObtenerAyudaAsync(1, "OrdenServicio", "cmbVehiculos");
                Assert.NotNull(ayuda);
                Assert.Equal(1, ayuda!.IdAyuda);
                Assert.Single(ayuda.Secciones);
            }
            finally { File.Delete(path); }
        }

        [Fact]
        public async Task ObtenerAyudaAsync_RolInexistente_DevuelveNull()
        {
            var path = CrearJsonTemp();
            try
            {
                var svc = new AyudaService(path);
                var ayuda = await svc.ObtenerAyudaAsync(99, "OrdenServicio", "cmbVehiculos");
                Assert.Null(ayuda);
            }
            finally { File.Delete(path); }
        }

        [Fact]
        public async Task ObtenerAyudaAsync_ArchivoInexistente_DevuelveNullSinExcepcion()
        {
            var svc = new AyudaService(Path.Combine(Path.GetTempPath(), "no_existe.json"));
            var ayuda = await svc.ObtenerAyudaAsync(1, "OrdenServicio", "cmbVehiculos");
            Assert.Null(ayuda);
        }

        [Fact]
        public async Task ObtenerAyudasPantallaAsync_FiltraPorPantalla()
        {
            var path = CrearJsonTemp();
            try
            {
                var svc = new AyudaService(path);
                var lista = await svc.ObtenerAyudasPantallaAsync(1, "OrdenServicio");
                Assert.Single(lista);
            }
            finally { File.Delete(path); }
        }
    }
}