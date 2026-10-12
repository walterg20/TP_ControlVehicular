import io

with io.open('Presentacion/ViewModels/BaseViewModel.cs', 'r', encoding='utf-8') as f:
    content = f.read()

props = '''
        protected virtual string Modulo => string.Empty;

        public bool PuedeCrear => string.IsNullOrEmpty(Modulo) || TienePermiso($"{Modulo}.Crear");
        public bool PuedeEditar => string.IsNullOrEmpty(Modulo) || TienePermiso($"{Modulo}.Editar");
        public bool PuedeEliminar => string.IsNullOrEmpty(Modulo) || TienePermiso($"{Modulo}.Eliminar");

        public bool TienePermiso(string permiso)
        {
            var main = System.Windows.Application.Current?.MainWindow as MainWindow;
            if (main == null || main.UsuarioSesionActual == null || main.UsuarioSesionActual.Permisos == null) 
                return true; 
            return main.UsuarioSesionActual.Permisos.Contains(permiso);
        }

        public void NotificarPermisos()
        {
            OnPropertyChanged(nameof(PuedeCrear));
            OnPropertyChanged(nameof(PuedeEditar));
            OnPropertyChanged(nameof(PuedeEliminar));
        }
'''

content = content.replace(
    'public bool HasErrors => _errors.Values.Any(list => list.Count > 0);',
    'public bool HasErrors => _errors.Values.Any(list => list.Count > 0);\n' + props
)

with io.open('Presentacion/ViewModels/BaseViewModel.cs', 'w', encoding='utf-8') as f:
    f.write(content)