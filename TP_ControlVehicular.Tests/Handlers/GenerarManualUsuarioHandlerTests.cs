// walterg20
using System.IO;
using TP_ControlVehicular.Negocio.Handlers.Reportes;
using Xunit;

namespace TP_ControlVehicular.Tests.Handlers
{
    public class GenerarManualUsuarioHandlerTests
    {
        [Fact]
        public void Handle_DeberiaGenerarPdf()
        {
            var handler = new GenerarManualUsuarioHandler();
            var path = handler.Handle("1.0.0-test");

            Assert.False(string.IsNullOrWhiteSpace(path));
            Assert.True(File.Exists(path));
            Assert.EndsWith(".pdf", path);

            // cleanup
            try { File.Delete(path); } catch { }
        }
    }
}
