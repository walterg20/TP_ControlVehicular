# Constitución del Proyecto (TP_ControlVehicular)

## Principios Innegociables
1. **Desacoplamiento Estricto UI/Lógica:** Todo el diseño va en XAML. La lógica de negocio y presentación va en los ViewModels que heredan de BaseViewModel usando comandos (RelayCommand). No se permite lógica de negocio en Ctl*.xaml.cs.
2. **Especificación Continua (OpenSpec):** No se escribe código de producción sin un documento spec.md y un plan de tareas en la carpeta openspec/. Todo nuevo feature requiere pasar por el proceso OpenSpec.
3. **Flujo de Ramas (Git Branching):** Para cada especificación (spec.md / Feature) SE DEBE CREAR UNA RAMA en Git. El trabajo (commits) se hace en esa rama hasta que se complete y verifique el feature. Recién entonces se une (merge) a la rama principal (develop/main).
4. **Integridad de Codificación (UTF-8 Estricto):** ESTÁ ESTRICTAMENTE PROHIBIDO modificar archivos con > o >> o Set-Content en PowerShell para evitar corromper tildes y emojis. Para modificar archivos de forma segura, los agentes DEBEN usar scripts de Python (.py) ejecutados con python o usar las herramientas nativas de reescritura. Todo script .py temporal debe colocarse en docs/script.
5. **Avisos y Confirmaciones Nativas:** Prohibido usar MessageBox.Show. Para alertas o confirmaciones, se invoca siempre a FrmConfirmacion.Mostrar() o MostrarAviso() localizado en Presentacion.Pantalla.Compartido.
6. **Exploración Inteligente:** Se DEBE utilizar **CodeGraph** y el MCP correspondiente para comprender o localizar código antes de recurrir a búsquedas rudimentarias.