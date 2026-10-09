import os
import re

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\ViewModels\OrdenServicioViewModel.cs'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

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

catch_target = '''                RegistrationFailed?.Invoke(this, "Error al guardar la orden de servicio:\\n" + current.Message);
                return false;
            }
        }'''
catch_repl = '''                RegistrationFailed?.Invoke(this, "Error al guardar la orden de servicio:\\n" + current.Message);
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
