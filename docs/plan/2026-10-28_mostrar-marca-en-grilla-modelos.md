# Plan de Ejecución: Corrección de Visualización de Marca en Grilla de Modelos

**Fecha:** 2026-10-28  
**Autor:** Antigravity  
**Contexto:** La columna "Marca" en `CtlModelo.xaml` no visualiza el nombre de la marca asociada al modelo.

## 1. Diagnóstico y Causa Raíz

En [`CtlModelo.xaml`](file:///e:/UNNE/Taller%20de%20Programaci%C3%B3n%20II/Proyecto/TP_ControlVehicular/Presentacion/Pantalla/Modelo/CtlModelo.xaml#L95), la grilla contiene:
```xml
<DataGridTextColumn Header="Marca" Binding="{Binding MarcaNombre}" Width="200"/>
```

Al consultar la lista de modelos:
1. `ModeloViewModel.LoadAsync()` ejecuta `_listarModelosHandler.HandleAsync()`.
2. `ListarModelosHandler` invoca `_modeloRepository.GetAllAsync()`.
3. `ModeloRepository` no sobreescribía `GetAllAsync()`, por lo que se ejecutaba `Repository<T>.GetAllAsync()`. Esta consulta base hace `_dbSet.AsNoTracking().ToListAsync()` sin `.Include(m => m.Marca)`.
4. Por omisión de *eager loading*, EF Core deja `Modelo.Marca = null`.
5. En `MappingProfile.cs`:
   ```csharp
   CreateMap<Modelo, ModeloDto>()
       .ForMember(dest => dest.IdMarca, opt => opt.MapFrom(src => src.MarcaId))
       .ForMember(dest => dest.MarcaNombre, opt => opt.MapFrom(src => src.Marca != null ? src.Marca.NombreMarca : string.Empty));
   ```
   Al ser `src.Marca == null`, `MarcaNombre` queda como `string.Empty` en todos los DTOs.
6. Lo mismo ocurre en `RegistrarModeloHandler` y `ModificarModeloHandler`, donde se invoca `_modeloRepository.GetByIdAsync(entity.Id)`, el cual tampoco realizaba `.Include(m => m.Marca)`.
7. Adicionalmente, `ModeloRepository.GetByMarcaAsync(int idMarca)` contenía un error en la condición (`m.Id == idMarca` en lugar de `m.MarcaId == idMarca`).

## 2. Plan de Acción

1. **`Repository<T>`**:
   - Declarar `GetAllAsync()`, `GetTodosAsync()` y `GetByIdAsync(int id)` como `virtual` para habilitar especialización polimórfica en repositorios hijos que requieran incluir grafos de entidades asociadas.

2. **`ModeloRepository`**:
   - Sobreescribir `GetAllAsync()`, `GetTodosAsync()` y `GetByIdAsync(int id)` agregando `.Include(m => m.Marca).AsNoTracking()`.
   - Corregir `GetByMarcaAsync(int idMarca)` incorporando `.Include(m => m.Marca)` y filtrando adecuadamente por `m.MarcaId == idMarca`.

3. **Pruebas Unitarias (`TP_ControlVehicular.Tests`)**:
   - Incorporar tests en `HandlersUnitTests.cs` para validar que `ListarModelosHandler`, `RegistrarModeloHandler` y `ModificarModeloHandler` devuelven `ModeloDto` con su propiedad `MarcaNombre` poblada cuando la entidad posee su objeto `Marca`.

4. **Verificación**:
   - Compilación: `dotnet build "TP_ControlVehicular.slnx"`
   - Tests: `dotnet test "TP_ControlVehicular.slnx"`
