import os
import glob

ROOT = r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular"
COMMENT = "// walterg20\n"

count = 0
for path in glob.glob(os.path.join(ROOT, "**", "*.cs"), recursive=True):
    if "\\bin\\" in path or "\\obj\\" in path:
        continue
    try:
        with open(path, "r", encoding="utf-8") as f:
            content = f.read()
    except UnicodeDecodeError:
        with open(path, "r", encoding="latin-1") as f:
            content = f.read()
    if content.startswith(COMMENT) or content.startswith("// walterg20"):
        continue
    bom = ""
    if content.startswith("\ufeff"):
        bom = "\ufeff"
        content = content[len(bom):]
    new_content = bom + COMMENT + content
    with open(path, "w", encoding="utf-8") as f:
        f.write(new_content)
    count += 1

print(f"Archivos actualizados: {count}")
