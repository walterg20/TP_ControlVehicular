import pathlib

BASE = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")
p = BASE / "MEMORY.md"

d = p.read_text(encoding="utf-8")

viejo_inicio = "- **Paginación en BD (spec `paginacion-listados`, Fases 1, 2 y 3 completas)**"
i = d.find(viejo_inicio)
if i == -1:
    print("ERROR: no se encontro la entrada de la spec en MEMORY.md")
    raise SystemExit(1)

# La entrada termina en la linea siguiente (empieza con "- Rama de trabajo anterior").
j = d.find("\n- Rama de trabajo anterior:", i)
if j == -1:
    print("ERROR: no se encontro el fin de la entrada")
    raise SystemExit(1)

nuevo = (
    "- **Paginación en BD (spec `paginacion-listados`, Fases 1, 2 y 3 completas)**: infraestructura compartida "
    "`IPaginacion` + `BasePaginableViewModel` (`Presentacion/ViewModels/`), `PaginacionViewModel` para pantallas "
    "con varias grillas paginadas, control reutilizable `CtlPaginacion` (`Presentacion/Pantalla/Compartido/`) y helper "
    "`Debouncer` (`Presentacion/Helpers/`, 500 ms). Listados migrados: Reporte Gerencial (las 4 pestañas: Ingresos, "
    "Top Clientes, Modelos más reparados y Servicios por rango), Clientes, Marcas, Modelos, Vehículos, Roles, "
    "Usuarios, Servicios, Talleres, Órdenes de Trabajo, Mis Trabajos (maestro), Reporte Órdenes y Reportes de "
    "Recepción. SPs en `bd/03_procedimientos.sql`: `sp_ReporteOrdenesPaginado` (con `STRING_AGG`), "
    "`sp_ReporteTopClientesPaginado`, `sp_ReporteModelosReparadosPaginado`, `sp_ReporteServiciosPorRangoPaginado`, "
    "`sp_HistorialVehiculoPaginado` y `sp_VehiculosPorClientePaginado` (con `@TamanoPagina = 0` devuelven el "
    "conjunto completo para el PDF). Los PDFs de reporte siguen exportando el conjunto filtrado completo. "
    "87 tests en verde (11 E2E de paginación + 6 E2E del reporte gerencial).\n"
    "- **Pestañas del Reporte Gerencial**: las tres ranking (Top Clientes, Modelos más reparados, Servicios por rango) "
    "dejaron de limitarse con `TOP 10` y ahora muestran el ranking completo paginado; sus PDF también exportan el "
    "ranking completo. Los SP originales con `@TopN` se conservan porque `ReporteOperativoViewModel` los sigue "
    "usando con top 10."
)

d = d[:i] + nuevo + d[j:]
p.write_text(d, encoding="utf-8", newline="")
print("OK: MEMORY.md actualizado")
