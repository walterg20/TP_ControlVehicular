# Specification: Manual de Usuario en PDF

## Capability: `manual-usuario-pdf`

### Scenario 1: Generación del manual
```gherkin
Given the user clicks "Ayuda → Manual de Usuario"
When the handler runs
Then a PDF file "Manual_Usuario_v{version}.pdf" is created
And the file is opened in the default PDF viewer
```

### Scenario 2: Contenido por rol
```gherkin
Given the generated PDF
When the user reads it
Then it contains sections for Login, Clientes/Vehículos, Órdenes de Servicio, Mis Trabajos, Reportes, Configuración de empresa
And each section includes step-by-step instructions
```

### Scenario 3: Encabezado institucional
```gherkin
Given the PDF is generated
When viewing any page
Then it shows the company logo and header via ReporteExtensions
And a footer with page numbers
```

### Scenario 4: Versionado
```gherkin
Given the application version is X.Y.Z
When the manual is generated
Then the filename includes "vX.Y.Z"
And the cover page displays the version and build date
```

### Scenario 5: Build verification
```gherkin
Given all changes are implemented
When building the solution
Then `dotnet build "TP_ControlVehicular.slnx"` completes with 0 errors
```
