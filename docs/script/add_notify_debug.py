import os

ROOT = r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular"
count = 0
for dirpath, _, files in os.walk(os.path.join(ROOT, "Presentacion")):
    for fn in files:
        if not fn.endswith(".cs"):
            continue
        path = os.path.join(dirpath, fn)
        with open(path, "r", encoding="utf-8") as f:
            lines = f.readlines()
        new_lines = []
        ctx = fn.replace(".cs", "")
        for line in lines:
            new_lines.append(line)
            if "Debug.WriteLine" in line and "IErrorNotifier" not in line:
                indent = line[:len(line) - len(line.lstrip())]
                # try to extract variable name 'ex' - assume ex
                new_lines.append(f'{indent}(App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "{ctx}");\n')
                count += 1
        if new_lines != lines:
            with open(path, "w", encoding="utf-8") as f:
                f.writelines(new_lines)

# also Negocio services
for dirpath, _, files in os.walk(os.path.join(ROOT, "Negocio")):
    for fn in files:
        if not fn.endswith(".cs"):
            continue
        path = os.path.join(dirpath, fn)
        with open(path, "r", encoding="utf-8") as f:
            lines = f.readlines()
        new_lines = []
        ctx = fn.replace(".cs", "")
        for line in lines:
            new_lines.append(line)
            if "Debug.WriteLine" in line:
                indent = line[:len(line) - len(line.lstrip())]
                new_lines.append(f'{indent}(App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "{ctx}");\n')
                count += 1
        if new_lines != lines:
            with open(path, "w", encoding="utf-8") as f:
                f.writelines(new_lines)

print("debug-notify replacements:", count)
PYEOF