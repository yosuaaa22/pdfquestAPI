using System.Linq.Expressions;

namespace pdfquestAPI.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // Mengambil satu item berdasarkan ID-nya.
        Task<T> GetByIdAsync(int id);

        // Mengambil semua item dari satu jenis.
        Task<IEnumerable<T>> GetAllAsync();

        // Menambahkan item baru (belum disimpan ke database).
        Task AddAsync(T entity);

        // Menandai item untuk dihapus (belum dihapus dari database).
        void Delete(T entity);

        // Menandai item untuk diubah (belum diubah di database).
        void Update(T entity);
    }
}