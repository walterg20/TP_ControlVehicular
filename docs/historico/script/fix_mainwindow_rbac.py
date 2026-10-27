import io
import re

with io.open('Presentacion/MainWindow.xaml.cs', 'r', encoding='utf-8') as f:
    content = f.read()

replacement = '''
        private void AplicarRestriccionesPorRol()
        {
            if (UsuarioSesionActual == null || UsuarioSesionActual.Permisos == null) return;
            var permisos = UsuarioSesionActual.Permisos;

            // Mostrar u ocultar menús según permisos de "Ver"
            secAdministracion.Visibility = (permisos.Contains("Usuario.Ver") || permisos.Contains("Taller.Ver") || permisos.Contains("Vehiculo.Ver")) ? Visibility.Visible : Visibility.Collapsed;
            
            btnUsuario.Visibility = permisos.Contains("Usuario.Ver") ? Visibility.Visible : Visibility.Collapsed;
            btnRol.Visibility = permisos.Contains("Usuario.Ver") ? Visibility.Visible : Visibility.Collapsed; 
            btnTaller.Visibility = permisos.Contains("Taller.Ver") ? Visibility.Visible : Visibility.Collapsed;
            
            btnCliente.Visibility = permisos.Contains("Cliente.Ver") ? Visibility.Visible : Visibility.Collapsed;
            btnVehiculo.Visibility = permisos.Contains("Vehiculo.Ver") ? Visibility.Visible : Visibility.Collapsed;
            btnModelo.Visibility = permisos.Contains("Vehiculo.Ver") ? Visibility.Visible : Visibility.Collapsed;
            btnMarca.Visibility = permisos.Contains("Vehiculo.Ver") ? Visibility.Visible : Visibility.Collapsed;
            
            btnServicio.Visibility = permisos.Contains("Servicio.Ver") ? Visibility.Visible : Visibility.Collapsed;
            btnOrdenesTrabajo.Visibility = permisos.Contains("OrdenServicio.Ver") ? Visibility.Visible : Visibility.Collapsed;
            btnMisTrabajos.Visibility = (permisos.Contains("OrdenServicio.Ver") || UsuarioSesionActual.IdRol == 3) ? Visibility.Visible : Visibility.Collapsed; // Especial para mecánicos u otros que ven órdenes
            
            bool puedeVerReportes = permisos.Contains("Reporte.Ver");
            secReportes.Visibility = puedeVerReportes ? Visibility.Visible : Visibility.Collapsed;
            btnReporteOrdenes.Visibility = puedeVerReportes ? Visibility.Visible : Visibility.Collapsed;
            btnReporteGerencial.Visibility = puedeVerReportes ? Visibility.Visible : Visibility.Collapsed;
            btnReporteOperativo.Visibility = puedeVerReportes ? Visibility.Visible : Visibility.Collapsed;
        }

        private void MenuItem_Click_Dashboard'''

content = re.sub(r'private void AplicarRestriccionesPorRol\(\)[\s\S]*?private void MenuItem_Click_Dashboard', replacement, content)

with io.open('Presentacion/MainWindow.xaml.cs', 'w', encoding='utf-8') as f:
    f.write(content)