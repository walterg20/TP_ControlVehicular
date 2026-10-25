import io

with io.open('Presentacion/Pantalla/Rol/CtlRol.xaml.cs', 'r', encoding='utf-8') as f:
    cs_content = f.read()

cs_replacement = '''
        private void BtnAdministrarPermisos_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RolViewModel vmRol)
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
        }
'''

cs_content = cs_content.replace('private async void BtnBaja_Click', cs_replacement + '\n        private async void BtnBaja_Click')

with io.open('Presentacion/Pantalla/Rol/CtlRol.xaml.cs', 'w', encoding='utf-8') as f:
    f.write(cs_content)