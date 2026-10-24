import os, glob

ROOT = r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular"
COMMENT = "// walterg20"

count = 0
for path in glob.glob(os.path.join(ROOT, "**", "*.cs"), recursive=True):
    if "\\bin\\" in path or "\\obj\\" in path:
        continue
    try:
        with open(path, "r", encoding="utf-8-sig") as f:
            content = f.read()
    except UnicodeDecodeError:
        with open(path, "r", encoding="latin-1") as f:
            content = f.read()
    content = content.replace("\ufeff", "")
    lines = content.splitlines(keepends=True)
    i = 0
    while i < len(lines) and (lines[i].strip() == COMMENT or lines[i].strip() == ""):
        if lines[i].strip() == COMMENT:
            i += 1
        elif lines[i].strip() == "" and i == 0:
            i += 1
        else:
            break
    new_body = "".join(lines[i:])
    new_content = COMMENT + "\n" + new_body.lstrip("\n")
    with open(path, "w", encoding="utf-8") as f:
        f.write(new_content)
    count += 1

print(f"Archivos normalizados: {count}")
