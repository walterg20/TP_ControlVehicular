import pathlib
import subprocess

base = pathlib.Path(r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular")

salida = subprocess.run(
    ["git", "status", "--porcelain"], cwd=base, capture_output=True, text=True
).stdout.splitlines()

paths = []
for linea in salida:
    p = linea[3:].strip().strip('"')
    if p.endswith("/"):
        paths += [str(x.relative_to(base)) for x in (base / p).rglob("*") if x.is_file()]
    else:
        paths.append(p)

for extra in (
    "AGENTS.md",
    "MEMORY.md",
    "docs/plan/2026-10-29_paginacion-listados.md",
    "openspec/changes/paginacion-listados/tasks.md",
):
    if extra not in paths:
        paths.append(extra)

for t in paths:
    p = base / t
    if not p.is_file():
        continue
    d = p.read_bytes()
    n = d.replace(b"\r\n", b"\n").replace(b"\n", b"\r\n")
    if n != d:
        p.write_bytes(n)
        print("CRLF ->", t)
