// walterg20
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TP_ControlVehicular.Negocio.Reportes.Documentos;

namespace TP_ControlVehicular.Negocio.Reportes.Documentos
{
    /// <summary>
    /// Manual de usuario del sistema de control vehicular, generado en PDF.
    /// </summary>
    public class ManualUsuarioDocument : IDocument
    {
        private readonly string _version;

        public ManualUsuarioDocument(string version)
        {
            _version = string.IsNullOrWhiteSpace(version) ? "1.0.0" : version;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.ComposeEncabezadoTaller("Manual de Usuario");
        }

        private void ComposeFooter(IContainer container)
        {
            container.ComposePieDePagina();
        }

        private void ComposeContent(IContainer container)
        {
            container.Column(col =>
            {
                col.Spacing(12);

                col.Item().Text($"Manual de Usuario - Sistema de Control Vehicular").FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                col.Item().Text($"Versión {_version}  |  Fecha de generación: {DateTime.Now:dd/MM/yyyy}").FontSize(10).Italic().FontColor(Colors.Grey.Darken1);
                col.Item().PaddingVertical(8).LineHorizontal(1).LineColor(Colors.Grey.Medium);

                Section(col, "1. Introducción",
                    "El Sistema de Control Vehicular permite gestionar clientes, vehículos, órdenes de servicio, tareas de taller y reportes. " +
                    "Este manual describe el uso del sistema por rol: Administrador, Mecánico y Recepcionista.");

                Section(col, "2. Acceso al sistema",
                    "1. Ingrese su usuario y contraseña en la pantalla de login.\n" +
                    "2. El sistema valida las credenciales y carga el menú según el rol asignado.\n" +
                    "3. Para cerrar sesión, use el botón \"Cerrar Sesión\" en el menú lateral.");

                Section(col, "3. Gestión de Clientes y Vehículos",
                    "Desde el menú Administración puede crear, editar, buscar y eliminar clientes, vehículos, modelos y marcas. " +
                    "Use la barra de búsqueda para filtrar por nombre, DNI o patente.");

                Section(col, "4. Órdenes de Servicio",
                    "En Órdenes de Trabajo puede:\n" +
                    "• Crear una nueva orden asociada a un cliente y vehículo.\n" +
                    "• Filtrar por rango de fechas (Desde / Hasta) y por estado (Todos, Abierta, En Proceso, Completada, Pagada, Cancelada).\n" +
                    "• Limpiar filtros con el botón \"Limpiar Filtros\".\n" +
                    "• Imprimir el listado filtrado a PDF con \"Imprimir Listado\".\n" +
                    "• Registrar servicios, repuestos y pagos.");

                Section(col, "5. Mis Trabajos (Mecánico)",
                    "El mecánico visualiza sus tareas asignadas, puede filtrar por fecha, generar la hoja de trabajo del día e imprimir el historial.");

                Section(col, "6. Reportes",
                    "Disponibles: Reporte de Órdenes, Reporte Gerencial, Reporte Operativo y Reporte de Servicios. " +
                    "Cada reporte permite filtrar por rango de fechas y exportar a PDF con membrete institucional.");

                Section(col, "7. Configuración de Empresa",
                    "El administrador puede configurar nombre, dirección, teléfono, email y logo de la empresa, que se reflejan en todos los reportes.");

                Section(col, "8. Preguntas Frecuentes",
                    "• ¿Olvidé mi contraseña? Contacte al administrador del sistema.\n" +
                    "• ¿Cómo imprimo un comprobante? Desde la orden de servicio, use \"Imprimir\".\n" +
                    "• ¿Dónde se guardan los PDFs? En la carpeta de documentos del usuario o en la ruta configurada.");

                Section(col, "9. Términos de Licencia y Soporte",
                    "El software se entrega bajo los términos de licencia acordados. El soporte técnico se proporciona según el plan contratado. " +
                    "No se asume responsabilidad por pérdida de datos por mal uso o falta de respaldo.");
            });
        }

        private static void Section(ColumnDescriptor col, string titulo, string texto)
        {
            col.Item().PaddingTop(8).Text(titulo).FontSize(14).Bold().FontColor(Colors.Blue.Darken1);
            col.Item().Text(texto).FontSize(11);
        }
    }
}
