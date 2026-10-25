// walterg20
using System.IO;
using QuestPDF.Fluent;
using TP_ControlVehicular.Negocio.Reportes.Documentos;

namespace TP_ControlVehicular.Negocio.Handlers.Reportes
{
    public class GenerarManualUsuarioHandler
    {
        public string Handle(string version)
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "TP_ControlVehicular", "Manuales");
            Directory.CreateDirectory(dir);

            var safeVersion = string.IsNullOrWhiteSpace(version) ? "1.0.0" : version;
            var fileName = $"Manual_Usuario_v{safeVersion}.pdf";
            var path = Path.Combine(dir, fileName);

            var doc = new ManualUsuarioDocument(safeVersion);
            doc.GeneratePdf(path);

            return path;
        }
    }
}
