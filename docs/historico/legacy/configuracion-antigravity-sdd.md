# Configuración Completa para Antigravity & OpenCode: Metodología SDD, TDD y Ecosistema Multiagente

> **Documento de Referencia y Plantillas de Proyecto**
> Generado a partir del análisis de la documentación del curso *"Curso de Desarrollo con IA: El Nuevo Programador"*.

---

## 1. Análisis de la Documentación del Curso

El paradigma actual del desarrollo de software impulsado por Inteligencia Artificial ha evolucionado desde el *prompting* básico o el *vibe coding* hacia la **Ingeniería de Arnés (Harness Engineering)** y la **Orquestación Multiagente basada en Especificaciones (SDD)**.

### Conceptos Clave
1. **El Arnés (Harness):** Un conjunto de estructuras, guardarraíles y contexto persistente que rodea al modelo LLM para asegurar que el agente trabaje bajo convenciones estrictas de producción sin salirse del camino.
2. **Spec-Driven Development (SDD):** Metodología de mitigación de riesgos frente al indeterminismo intrínseco de los modelos de lenguaje. En lugar de pedir código directamente, se construye una cadena formal: *Constitución → Especificación (spec.md) → Clarificación (QA) → Planificación (plan.md) → Tareas (tasks.md) → Implementación → Validación*.
3. **Test-Driven Development (TDD):** Principio innegociable en el flujo de implementación. El agente escribe primero los tests unitarios/integración en rojo (fallidos) y posteriormente desarrolla el código mínimo necesario para poner la suite de pruebas en verde.
4. **Ecosistema Multiagente:** Separación de responsabilidades (*Separation of Concerns*) en ventanas de contexto independientes. Evita la saturación del contexto utilizando un agente coordinador principal que delega tareas específicas a subagentes especializados (Planificador, Implementador, Revisor).
5. **MCP (Model Context Protocol):** Estándar abierto que conecta al agente en tiempo real con herramientas externas (navegador Chrome, documentación oficial Context7, GitHub, bases de datos).

---

## 2. Plantilla de Arnés Principal: `AGENTS.md`

Guarda este archivo en la raíz de tu proyecto como `AGENTS.md`. Actúa como el *system prompt* persistente para Antigravity y OpenCode.

```markdown
# AGENTS.md — [Nombre de tu Proyecto]

[Descripción clara en 1-2 frases del proyecto y su propósito.]

## Stack Tecnológico
- Lenguaje: TypeScript (modo estricto) / Node.js
- Framework: Next.js / Express
- Base de datos: PostgreSQL con Prisma / Supabase
- Testing: Vitest / Node Test Runner (`node --test`)
- Estilos: Tailwind CSS

## Comandos Clave
- `npm run dev`      — Inicia el servidor de desarrollo local.
- `npm test`         — Ejecuta las pruebas unitarias e integración.
- `npm run lint`     — Valida el estilo y convenciones del código.
- `npm run build`    — Compila el proyecto para producción.

## Estructura del Proyecto
- `src/core/`       — Lógica de negocio pura (funciones puras independientes del UI).
- `src/components/` — Componentes de interfaz de usuario.
- `specs/`          — Especificaciones SDD (spec.md, plan.md, tasks.md).
- `docs/`           — Constitución del proyecto y reglas globales.
- `tests/`          — Suites de pruebas TDD.

## Reglas y Convenciones
- **TDD Estricto:** Toda función de lógica de negocio debe contar con sus pruebas asociadas escritas antes de la implementación.
- **Separación de Capas:** No acoplar lógica de datos ni funciones de tiempo dentro de los componentes visuales.
- **Idioma:** Código y variables en inglés; comentarios y documentación en español.

## Flujo de Trabajo y Límites
- ✅ **Siempre:** Leer `docs/constitution.md` y `MEMORY.md` antes de proponer o ejecutar cambios.
- ⚠ **Pregunta antes de:** Modificar esquemas de base de datos o agregar librerías de terceros.
- 🚫 **Nunca:** Modificar código sin una prueba que valide el comportamiento o romper la compatibilidad existente.

## Memoria
- Al iniciar una sesión, lee `MEMORY.md` para conocer el estado actual y contexto reciente.
- Al finalizar una tarea relevante, actualiza `MEMORY.md` con el estado, decisiones técnicas tomadas y trampas a evitar.
```

---

## 3. Plantilla de Constitución: `docs/constitution.md`

Crear en `docs/constitution.md`. Define las reglas innegociables del proyecto.

```markdown
# Constitución del Proyecto

## Principios Innegociables
1. **Simplicidad del Stack:** Preferir soluciones estándar del ecosistema antes que dependencias externas innecesarias.
2. **Especificación sobre Código:** No se escribe código de producción sin una especificación (`spec.md`) aprobada previa.
3. **Desacoplamiento UI / Lógica:** La lógica de negocio reside en funciones puras independientes de la capa de presentación.
4. **Política TDD:** Los tests se escriben primero (estado rojo) y se ejecutan continuamente mediante la suite del proyecto.
5. **Protección de Datos:** Prohibido almacenar claves, secretos, tokens o datos personales en el código o repositorios.
6. **Claridad Idiomática:** Código en inglés, interfaz y documentación de usuario en español.
```

---

## 4. Definición de Agentes y Subagentes (`.opencode/agents/`)

### A. Coordinador Principal (`.opencode/agents/coordinator.md`)
```markdown
---
description: Coordina el flujo SDD completo gestionando los subagentes planner, implementer y reviewer.
mode: primary
permissions:
  - action: edit
    resource: "*"
    effect: allow
  - action: shell
    resource: "*"
    effect: allow
  - action: subagent
    resource: "*"
    effect: allow
---

Eres el agente Coordinador. Tu responsabilidad es dirigir la orquestación multiagente siguiendo el flujo SDD:
1. Pide a @planner que redacte la especificación (`spec.md`).
2. Pide a @reviewer la revisión QA de la spec.
3. Pide a @planner el plan técnico (`plan.md`) y el desglose de tareas (`tasks.md`).
4. Delega a @implementer la ejecución TDD tarea por tarea.
5. Pide a @reviewer la validación final del proyecto.
```

### B. Subagente Planificador (`.opencode/agents/planner.md`)
```markdown
---
description: Redacta especificaciones EARS, planes técnicos y desglose de tareas en specs/ sin modificar código.
mode: subagent
permissions:
  - action: edit
    resource: "specs/**"
    effect: allow
  - action: edit
    resource: "*"
    effect: deny
  - action: shell
    resource: "*"
    effect: deny
---

Eres el agente Planificador (@planner). Diseñas las specs siguiendo el formato EARS, los planes técnicos con decisiones justificadas y los listados de tareas atómicas (`tasks.md`). Nunca modificas código de producción.
```

### C. Subagente Implementador TDD (`.opencode/agents/implementer.md`)
```markdown
---
description: Ejecuta tareas bajo metodología TDD (tests primero en rojo, luego código en verde).
mode: subagent
permissions:
  - action: edit
    resource: "*"
    effect: allow
  - action: shell
    resource: "*"
    effect: allow
---

Eres el agente Implementador (@implementer). Trabajas bajo TDD estricto:
1. Lees la tarea asignada en `specs/NNN/tasks.md`.
2. Escribes los tests unitarios correspondientes (compruebas que fallen en rojo).
3. Escribes el código de producción necesario para pasar las pruebas (estado verde).
4. Ejecutas la suite de comandos de verificación antes de marcar la tarea como hecha.
```

### D. Subagente Revisor QA (`.opencode/agents/reviewer.md`)
```markdown
---
description: Audita especificaciones y valida la implementación final contra los criterios de aceptación.
mode: subagent
permissions:
  - action: edit
    resource: "*"
    effect: deny
  - action: shell
    resource: "*"
    effect: allow
---

Eres el agente Revisor QA (@reviewer). Evalúas las specs detectando ambigüedades o contradicciones y verificas el código final comprobando que cumple todos los Requisitos Funcionales (RF) definidos.
```

---

## 5. Configuración de MCPs Esenciales (`mcp.json`)

```json
{
  "mcpServers": {
    "context7": {
      "command": "npx",
      "args": ["-y", "@upstash/context7-mcp"]
    },
    "chrome-devtools": {
      "command": "npx",
      "args": ["-y", "@chrome-devtools/mcp-server"]
    },
    "github": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-github"]
    }
  }
}
```

---

## 6. Instrucciones para Otorgar Permisos Totales (Modo Sin Interrupciones)

Si deseas trabajar sin confirmaciones constantes ("que no pida permiso a cada rato"):

1. **Configuración por Agente (YAML Header):** En cada archivo de definición de agente en `.opencode/agents/*.md`, establece el campo `effect: allow` en los recursos globales:
   ```yaml
   permissions:
     - action: edit
       resource: "*"
       effect: allow
     - action: shell
       resource: "*"
       effect: allow
     - action: subagent
       resource: "*"
       effect: allow
     - action: webfetch
       resource: "*"
       effect: allow
   ```
2. **Modo de Ejecución en IDE/Terminal:**
   - En **OpenCode**, ejecuta tus comandos en modo **Build** o desactiva la confirmación de comandos shell mediante la configuración global de permisos.
   - En **Antigravity IDE**, selecciona la opción de auto-aprobación para herramientas conocidas (*Tool Auto-Approve*) en la barra de ajustes del agente.
