import os

ROOT = r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular"
count = 0
for dirpath, _, files in os.walk(os.path.join(ROOT, "Presentacion")):
    for fn in files:
        if not fn.endswith(".cs"):
            continue
        path = os.path.join(dirpath, fn)
        with open(path, "r", encoding="utf-8") as f:
            src = f.read()
        if "IErrorNotifier" not in src:
            continue
        if "using TP_ControlVehicular.Negocio.Services;" in src:
            continue
        # add after first using line
        lines = src.split("\n")
        # find last using
        idx = 0
        for i, line in enumerate(lines):
            if line.strip().startswith("using "):
                idx = i
        lines.insert(idx + 1, "using TP_ControlVehicular.Negocio.Services;")
        with open(path, "w", encoding="utf-8") as f:
            f.write("\n".join(lines))
        count += 1
print("files updated:", count)
PYEOF