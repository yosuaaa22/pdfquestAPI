using pdfquestAPI.Data; 
using pdfquestAPI.Interfaces;
using pdfquestAPI.Models;
using System.Threading.Tasks;

namespace pdfquestAPI.Repositories
{
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

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
