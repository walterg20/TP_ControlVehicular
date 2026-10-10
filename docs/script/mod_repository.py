import os
import re

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Datos\Repositories\RegistroServicioRepository.cs'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

target = '''        public new async Task UpdateAsync(RegistroServicio entity)
        {
            _cvDbContext.ChangeTracker.Clear();
            _cvDbContext.RegistroServicios.Update(entity);
            await _cvDbContext.SaveChangesAsync();
        }'''

repl = '''        public new async Task UpdateAsync(RegistroServicio entity)
        {
            _cvDbContext.ChangeTracker.Clear();
            
            var existente = await _cvDbContext.RegistroServicios
                .Include(r => r.Detalles)
                .FirstOrDefaultAsync(r => r.Id == entity.Id);

            if (existente != null)
            {
                _cvDbContext.Entry(existente).CurrentValues.SetValues(entity);

                // Eliminar detalles que ya no estn
                foreach (var detalleExistente in existente.Detalles.ToList())
                {
                    if (!entity.Detalles.Any(d => d.Id == detalleExistente.Id))
                    {
                        _cvDbContext.DetalleServicios.Remove(detalleExistente);
                    }
                }

                // Agregar o actualizar detalles
                foreach (var detalleModel in entity.Detalles)
                {
                    var detalleExistente = existente.Detalles.FirstOrDefault(d => d.Id == detalleModel.Id);
                    if (detalleExistente != null)
                    {
                        _cvDbContext.Entry(detalleExistente).CurrentValues.SetValues(detalleModel);
                    }
                    else
                    {
                        // Asegurarse de que el ID del registro de servicio coincida
                        detalleModel.RegistroServicioId = existente.Id;
                        existente.Detalles.Add(detalleModel);
                    }
                }
                
                await _cvDbContext.SaveChangesAsync();
            }
            else
            {
                _cvDbContext.RegistroServicios.Update(entity);
                await _cvDbContext.SaveChangesAsync();
            }
        }'''

content = content.replace(target, repl)
with open(path, 'w', encoding='utf-8-sig') as f:
    f.write(content)
