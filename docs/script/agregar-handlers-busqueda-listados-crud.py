"""Agrega a las pantallas CRUD los handlers de busqueda server-side y el boton Buscar.

- Inserta en el code-behind los handlers TxtBusqueda_TextChanged, TxtBusqueda_KeyDown,
  BtnBuscar_Click y ChkMostrarInactivos_Click.
- Inserta en el XAML el boton "Buscar" donde falte.
"""

import pathlib
import re
import sys

BASE = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")

# (carpeta, xaml, code-behind, tipo de ViewModel, variable)
PANTALLAS = [
    ("Marca", "CtlMarca", "MarcaViewModel", "vmMarca"),
    ("Modelo", "CtlModelo", "ModeloViewModel", "vmModelo"),
    ("Vehiculo", "CtlVehiculo", "VehiculoViewModel", "vmVehiculo"),
    ("Rol", "CtlRol", "RolViewModel", "vmRol"),
    ("Usuario", "CtlUsuario", "UsuarioViewModel", "vmUsuario"),
    ("Servicio", "CtlServicio", "ServicioViewModel", "vmServicio"),
    ("Taller", "CtlTaller", "TallerViewModel", "vmTaller"),
]

PLANTILLA_HANDLERS = """
        /// <summary>
        /// Ejecuta la busqueda en la base de datos (boton Buscar).
        /// </summary>
        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {{
            if (this.DataContext is {vm_tipo} {var})
            {{
                {var}.CancelarBusquedaProgramada();
                await {var}.BuscarAsync();
            }}
        }}

        /// <summary>
        /// Busca con retardo mientras el usuario escribe, para no consultar la base de datos
        /// en cada tecla.
        /// </summary>
        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {{
            if (this.DataContext is {vm_tipo} {var})
            {{
                {var}.ProgramarBusqueda();
            }}
        }}

        /// <summary>
        /// La tecla Enter ejecuta la busqueda de inmediato.
        /// </summary>
        private async void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {{
            if (e.Key != Key.Enter) return;
            if (this.DataContext is {vm_tipo} {var})
            {{
                {var}.CancelarBusquedaProgramada();
                await {var}.BuscarAsync();
            }}
        }}

        /// <summary>
        /// Cambiar la visibilidad de los registros inactivos recarga el listado.
        /// </summary>
        private async void ChkMostrarInactivos_Click(object sender, RoutedEventArgs e)
        {{
            if (this.DataContext is {vm_tipo} {var})
            {{
                {var}.CancelarBusquedaProgramada();
                await {var}.BuscarAsync();
            }}
        }}
"""

BOTON_BUSCAR = """                    <Button Content="Buscar" Width="80" Padding="8,6" Margin="10,0,0,0" Background="#2980B9" Foreground="White" FontWeight="Bold" BorderThickness="0" Click="BtnBuscar_Click" Cursor="Hand">
                        <Button.Resources>
                            <Style TargetType="Border">
                                <Setter Property="CornerRadius" Value="6"/>
                            </Style>
                        </Button.Resources>
                    </Button>
"""


def agregar_handlers(contenido: str, vm_tipo: str, var: str) -> str:
    if "TxtBusqueda_TextChanged" in contenido:
        return contenido

    if "using System.Windows.Input;" not in contenido:
        contenido = contenido.replace(
            "using System.Windows.Controls;\n",
            "using System.Windows.Controls;\nusing System.Windows.Input;\n",
            1,
        )

    bloque = PLANTILLA_HANDLERS.format(vm_tipo=vm_tipo, var=var)

    # Si ya existe un BtnBuscar_Click (pantalla Taller), se reemplaza su cuerpo.
    patron_existente = re.compile(
        r'\n        private async void BtnBuscar_Click\(object sender, RoutedEventArgs e\)\n'
        r'        \{\n'
        r'.*?\n        \}\n',
        re.DOTALL,
    )
    if patron_existente.search(contenido):
        contenido = patron_existente.sub(bloque, contenido, count=1)
        return contenido

    # Si no existe, los handlers se insertan antes del primer boton.
    marca = re.search(r'\n        private (?:async )?void Btn', contenido)
    if not marca:
        raise RuntimeError("no se encontro un metodo Btn* donde insertar los handlers")

    return contenido[: marca.start()] + bloque + contenido[marca.start():]


def agregar_boton_buscar(contenido: str) -> str:
    if 'Click="BtnBuscar_Click"' in contenido:
        return contenido

    if contenido.count("<TextBox ") != 1:
        raise RuntimeError(f"se esperaba un solo TextBox y hay {contenido.count('<TextBox ')}")

    cierre = contenido.index("</TextBox>") + len("</TextBox>")
    return contenido[:cierre] + "\n" + BOTON_BUSCAR + contenido[cierre:]


def main() -> int:
    for carpeta, control, vm_tipo, var in PANTALLAS:
        xaml = BASE / "Presentacion" / "Pantalla" / carpeta / f"{control}.xaml"
        codebehind = BASE / "Presentacion" / "Pantalla" / carpeta / f"{control}.xaml.cs"

        contenido_xaml = xaml.read_text(encoding="utf-8")
        contenido_xaml = agregar_boton_buscar(contenido_xaml)
        xaml.write_text(contenido_xaml, encoding="utf-8", newline="")

        contenido_cs = codebehind.read_text(encoding="utf-8")
        contenido_cs = agregar_handlers(contenido_cs, vm_tipo, var)
        codebehind.write_text(contenido_cs, encoding="utf-8", newline="")

        print(f"OK: {carpeta}")

    return 0


if __name__ == "__main__":
    sys.exit(main())
