# Resaltado Dinámico del Menú Activo

## Overview
Actualmente en el menú lateral, el botón del `Dashboard` tiene un estilo quemado en el código (hardcoded) de color azul para indicar que es la pantalla activa. Al navegar hacia otras pantallas (como `Servicios` o `Clientes`), el botón del Dashboard sigue marcado en azul y el nuevo menú no se resalta. Se requiere que el indicador de "menú activo" siga a la selección del usuario.

## Functional Requirements
1. **Estilo del Botón Activo**: El botón que corresponda a la pantalla actual debe estar coloreado de azul (`#2980B9`) y con texto en negrita (`FontWeight="Bold"`).
2. **Estilo de los Botones Inactivos**: Los demás botones deben tener fondo transparente y fuente normal.
3. **Comportamiento Inicial**: Al iniciar sesión y entrar a la aplicación, el botón `Dashboard` debe resaltarse automáticamente por defecto.
4. **Navegación**: Al hacer clic en cualquier otro ítem del menú (por ejemplo, `Servicios`), su botón debe pintarse de azul y el anterior debe despintarse.

## Acceptance Criteria
- [ ] Al hacer clic en cualquier opción del menú lateral, el botón clickeado obtiene fondo azul.
- [ ] Simultáneamente, cualquier otro botón del menú pierde su fondo azul y vuelve a ser transparente.
- [ ] El cambio de colores no afecta los bordes redondeados (`CornerRadius`) aplicados a los botones.
