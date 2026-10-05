import os
import sys

def find_files(root_dir):
    targets = ["BillingService.cs", "CtlOrdenServicio.xaml"]
    for dirpath, dirnames, filenames in os.walk(root_dir):
        for f in filenames:
            if f in targets or "ViewModel" in f:
                print(os.path.join(dirpath, f))

if __name__ == "__main__":
    find_files("e:\\UNNE\\Taller de Programación II\\Proyecto\\TP_ControlVehicular")
