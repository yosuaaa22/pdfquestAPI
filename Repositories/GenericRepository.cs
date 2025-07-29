using Microsoft.EntityFrameworkCore;
using pdfquestAPI.Data; // Pastikan namespace DbContext Anda benar
using pdfquestAPI.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pdfquestAPI.Repositories
{
    // Ini adalah implementasi dari IGenericRepository.
    // Menggunakan ApplicationDbContext yang sudah kita buat.
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            // FindAsync lebih efisien untuk mencari berdasarkan Primary Key.
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Entity with id {id} was not found");
            return entity;
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
