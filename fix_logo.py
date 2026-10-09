import os
import re

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Negocio\Reportes\Documentos\ReporteExtensions.cs'
with open(path, 'r', encoding='utf-8') as f:
    content = f.read()

replacement = '''        public static void ComposeEncabezadoTaller(this IContainer container, string tituloReporte)
        {
            container.Row(row =>
            {
                // Logo a la izquierda
                row.ConstantItem(120).PaddingRight(10).AlignLeft().Image(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Presentacion", "Assets", "logo.png"));

                // Datos de la empresa al medio
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("TALLER PRO").FontSize(24).SemiBold().FontColor(Colors.Blue.Darken2);
                    col.Item().Text("Ruta Nacional 11 Km 1005 - Resistencia, Chaco").FontSize(10).FontColor(Colors.Grey.Darken1);
                    col.Item().Text("Tel: +54 362 4123456 | Email: contacto@taller.com").FontSize(10).FontColor(Colors.Grey.Darken1);
                });

                // Titulo del reporte a la derecha
                row.ConstantItem(150).AlignRight().Column(col =>
                {
                    col.Item().Text(tituloReporte).FontSize(16).Bold().FontColor(Colors.Black);
                    col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10);
                });
            });
        }'''

# Replace the existing method
content = re.sub(r'public static void ComposeEncabezadoTaller\(.*?\)\s*\{.*?\n        \}', replacement, content, flags=re.DOTALL)

with open(path, 'w', encoding='utf-8') as f:
    f.write(content)
