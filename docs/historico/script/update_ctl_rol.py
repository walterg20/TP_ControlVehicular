import io

with io.open('Presentacion/Pantalla/Rol/CtlRol.xaml', 'r', encoding='utf-8') as f:
    content = f.read()

replacement = '''<Button Content="🔑 Administrar Permisos" Click="BtnAdministrarPermisos_Click" Padding="12,8" Background="#8E44AD" Foreground="White" FontWeight="Bold" BorderThickness="0" Margin="0,0,8,0" Cursor="Hand">
                    <Button.Resources>
                        <Style TargetType="Border">
                            <Setter Property="CornerRadius" Value="6"/>
                        </Style>
                    </Button.Resources>
                </Button>
                <Button Content="✏️ Modificar Seleccionado"'''

content = content.replace('<Button Content="✏️ Modificar Seleccionado"', replacement)

with io.open('Presentacion/Pantalla/Rol/CtlRol.xaml', 'w', encoding='utf-8') as f:
    f.write(content)

with io.open('Presentacion/Pantalla/Rol/CtlRol.xaml.cs', 'r', encoding='utf-8') as f:
    cs_content = f.read()

cs_replacement = '''
        private void BtnAdministrarPermisos_Click(object sender, RoutedEventArgs e)
        {
            var rolSeleccionado = vmRol.RolSeleccionado;
            if (rolSeleccionado == null)
            {
                FrmConfirmacion.MostrarAviso("Por favor seleccione un rol para administrar sus permisos.", "Aviso", Window.GetWindow(this));
                return;
            }

            var frm = new TP_ControlVehicular.Presentacion.Pantalla.Rol.FrmRolPermiso(rolSeleccionado);
            if (frm.ShowDialog() == true)
            {
                _ = vmRol.LoadAsync();
            }
        }
'''

cs_content = cs_content.replace('private void BtnBaja_Click', cs_replacement + '\n        private void BtnBaja_Click')

with io.open('Presentacion/Pantalla/Rol/CtlRol.xaml.cs', 'w', encoding='utf-8') as f:
    f.write(cs_content)