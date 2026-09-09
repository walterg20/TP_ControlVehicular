using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Negocio.Interfaces;


namespace TP_ControlVehicular.Datos.Repositories
{
    public  class Repository<T> : IRepository<T> where T : class
    {
        private readonly CVDbContext _cvDbContext;
        private readonly DbSet<T> _dbSet;

        public Repository(CVDbContext cvDbContext)
        {
            _cvDbContext = cvDbContext;
            _dbSet = _cvDbContext.Set<T>();
        }
        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _cvDbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _cvDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _cvDbContext.SaveChangesAsync();
            }
        }
    }
}
