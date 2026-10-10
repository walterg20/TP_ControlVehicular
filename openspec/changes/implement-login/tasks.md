# Implementation Tasks: User Login

- [ ] 1. **Negocio Layer Implementation**
  - [ ] 1.1 Create `ResultadoAutenticacion` enum and `AuthResponseDto` in `Negocio/DTOs/`
  - [ ] 1.2 Implement `AutenticarUsuarioHandler` in `Negocio/Services/` using `IUsuarioRepository` and `IMapper`
  - [ ] 1.3 Add unit/db error capture logic (User Not Found vs Invalid Password vs Database Failure)

- [ ] 2. **Presentacion Layer Implementation**
  - [ ] 2.1 Create `LoginViewModel` in `Presentacion/ViewModels/` with validation and `OnLoginSuccess` event
  - [ ] 2.2 Create `CtlLogin.xaml` and `CtlLogin.xaml.cs` in `Presentacion/Pantalla/Login/` with WPF `CornerRadius="12"` (rounded-xl) and `DropShadowEffect` (shadow-md)
  - [ ] 2.3 Bind `CtlLogin` controls to `LoginViewModel`

- [ ] 3. **MainWindow & Navigation Integration**
  - [ ] 3.1 Modify `MainWindow.xaml` to name the navigation sidebar border (`pnlSidebar`) and adjust column layout dynamically
  - [ ] 3.2 Update `MainWindow.xaml.cs` to show `CtlLogin` and collapse sidebar on startup
  - [ ] 3.3 Handle `OnLoginSuccess` event in `MainWindow` to show sidebar menu and clear `CtlLogin`

- [ ] 4. **Dependency Injection & App Startup**
  - [ ] 4.1 Register `AutenticarUsuarioHandler`, `LoginViewModel`, and `CtlLogin` in `App.xaml.cs`
  - [ ] 4.2 Build and verify application compilation (`dotnet build`)
  - [ ] 4.3 Test login flow manually (`dotnet run`)
