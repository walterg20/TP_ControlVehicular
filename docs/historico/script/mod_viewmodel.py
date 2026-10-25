import os
import re

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\ViewModels\OrdenServicioViewModel.cs'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

# Replace the start of GuardarOrdenAsync
start_target = '''        public async Task<bool> GuardarOrdenAsync()
        {
            if (!ValidateAll()) return false;'''
start_repl = '''        public async Task<bool> GuardarOrdenAsync()
        {
            if (!ValidateAll()) return false;
            
            if (IsLoading) return false;
            IsLoading = true;
            try
            {'''
content = content.replace(start_target, start_repl)

# The end of the catch block in GuardarOrdenAsync looks something like this:
catch_target = '''                {
                    ex = ex.InnerException;
                }
                Presentacion.Pantalla.Compartido.FrmConfirmacion.MostrarAviso(
                    $"Error al guardar la orden:\n{ex.Message}", "Error de Base de Datos", null!);
                return false;
            }
        }'''
catch_repl = '''                {
                    ex = ex.InnerException;
                }
                Presentacion.Pantalla.Compartido.FrmConfirmacion.MostrarAviso(
                    $"Error al guardar la orden:\n{ex.Message}", "Error de Base de Datos", null!);
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }'''
content = content.replace(catch_target, catch_repl)

with open(path, 'w', encoding='utf-8-sig') as f:
    f.write(content)
