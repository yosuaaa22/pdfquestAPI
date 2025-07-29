using pdfquestAPI.Data; // Pastikan namespace DbContext Anda benar
using pdfquestAPI.Interfaces;
using pdfquestAPI.Models;
using System.Threading.Tasks;

namespace pdfquestAPI.Repositories
{
    // Ini adalah implementasi dari IUnitOfWork.
    // Dia yang membuat dan mengelola semua repository.
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        // Properti untuk setiap repository yang dibutuhkan aplikasi
        public IGenericRepository<JudulIsi> JudulIsi { get; private set; }
        public IGenericRepository<PenyediaLayanan> PenyediaLayanan { get; private set; }
        public IGenericRepository<Perjanjian> Perjanjian { get; private set; }
        public IGenericRepository<PihakPertama> PihakPertama { get; private set; }
        public IGenericRepository<PoinKetentuanKhusus> PoinKetentuanKhusus { get; private set; }
        public IGenericRepository<SubBabKetentuanKhusus> SubBabKetentuanKhusus { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            // Inisialisasi setiap repository
            JudulIsi = new GenericRepository<JudulIsi>(_context);
            PenyediaLayanan = new GenericRepository<PenyediaLayanan>(_context);
            Perjanjian = new GenericRepository<Perjanjian>(_context);
            PihakPertama = new GenericRepository<PihakPertama>(_context);
            PoinKetentuanKhusus = new GenericRepository<PoinKetentuanKhusus>(_context);
            SubBabKetentuanKhusus = new GenericRepository<SubBabKetentuanKhusus>(_context);
        }

        // Menyimpan semua perubahan yang terlacak ke database
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Melepaskan koneksi database
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
