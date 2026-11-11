import os, re

ROOT = r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular"
TARGETS = ["Presentacion"]

count = 0
for base in TARGETS:
    for dirpath, _, files in os.walk(os.path.join(ROOT, base)):
        for fn in files:
            if not fn.endswith(".cs"):
                continue
            path = os.path.join(dirpath, fn)
            with open(path, "r", encoding="utf-8") as f:
                src = f.read()

            ctx = fn.replace(".cs", "")
            pattern = re.compile(r"(\n\s*)catch\s*\n(\s*)\{")
            def repl(m):
                global count
                count += 1
                indent = m.group(2)
                nl = m.group(1)
                return (f"{nl}catch (Exception ex)\n{indent}{{\n{indent}    "
                        f"(App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)"
                        f"?.Notify(ex, \"{ctx}\");")
            new_src = pattern.sub(repl, src)

            if new_src != src:
                with open(path, "w", encoding="utf-8") as f:
                    f.write(new_src)

print("empty-catch replacements:", count)
PYEOF