# Design: Gestión de Visibilidad y Estado de Botones (RBAC)

## Arquitectura y Componentes Afectados

La forma más estandarizada en MVVM (WPF) de manejar permisos de interfaz es inyectando propiedades de solo lectura en los `ViewModels` y realizando un **Binding** desde los botones en el archivo `.xaml`.

### Estrategia de Implementación (Recomendada)

#### 1. En los ViewModels (`Ej: ClienteViewModel.cs`)
En lugar de escribir código espagueti en la vista, se añaden propiedades booleanas que encapsulan la regla de negocio del rol basándose en `UserSession.CurrentUser.IdRol`.

```csharp
public bool PuedeCrear => 
    UserSession.CurrentUser?.IdRol == (int)RolesSistema.Administrador || 
    UserSession.CurrentUser?.IdRol == (int)RolesSistema.Recepcionista;

public bool PuedeEliminar => 
    UserSession.CurrentUser?.IdRol == (int)RolesSistema.Administrador;
```
*(Se puede poner en el `BaseViewModel` de forma genérica, o en cada ViewModel específico si los permisos varían de pantalla a pantalla).*

#### 2. En las Vistas (`Ej: CtlCliente.xaml`)
Como el archivo `App.xaml` ya tiene definido el `BooleanToVisibilityConverter`, simplemente bindeamos esta propiedad a la visibilidad o al estado de "habilitado" del botón.

**Para ocultarlo (Opción más limpia visualmente):**
```xml
<Button Content="➕ Nuevo Cliente" 
        Command="{Binding NuevoCommand}"
        Visibility="{Binding PuedeCrear, Converter={StaticResource BooleanToVisibilityConverter}}" />
```

**Para deshabilitarlo (Mostrarlo en gris):**
```xml
<Button Content="➕ Nuevo Cliente" 
        Command="{Binding NuevoCommand}"
        IsEnabled="{Binding PuedeCrear}" />
```

### Alternativa: Validar mediante `CanExecute` de Commands
Si estás usando `RelayCommand`, otra manera es pasar el chequeo de permisos como el segundo parámetro (`CanExecute`).
```csharp
NuevoCommand = new RelayCommand(NuevoExecute, () => PuedeCrear);
```
Esto deshabilitará (pondrá en gris) el botón automáticamente en toda la interfaz sin tener que tocar el `.xaml`.

## Recomendación de Diseño
Para este proyecto sugerimos **combinar ambas**: 
Usar el Binding a `Visibility` para los botones superiores de "Nuevo" (si no puedes crear, no necesitas ver el botón) y el `CanExecute` en los comandos para asegurar que ni siquiera por atajo de teclado se pueda disparar la acción restringida.
