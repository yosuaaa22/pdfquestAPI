using pdfquestAPI.Models;

namespace pdfquestAPI.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Properti untuk mengakses setiap Repository yang spesifik.
        // disesuaikan dengan semua DbSet di ApplicationDbContext.
        IGenericRepository<JudulIsi> JudulIsi { get; }
        IGenericRepository<PenyediaLayanan> PenyediaLayanan { get; }
        IGenericRepository<Perjanjian> Perjanjian { get; }
        IGenericRepository<PihakPertama> PihakPertama { get; }
        IGenericRepository<PoinKetentuanKhusus> PoinKetentuanKhusus { get; }
        IGenericRepository<SubBabKetentuanKhusus> SubBabKetentuanKhusus { get; }

        // Metode untuk "menyimpan semua perubahan" ke database dalam satu transaksi.
        Task<int> CompleteAsync();
    }
}
