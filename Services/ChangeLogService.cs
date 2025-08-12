using pdfquestAPI.Data;
using pdfquestAPI.Interfaces;
using pdfquestAPI.Models;
using System.Threading.Tasks;

namespace pdfquestAPI.Services
{
    public class ChangeLogService : IChangeLogService
    {
        private readonly ApplicationDbContext _context;

        public ChangeLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(int perjanjianId, string pengguna, string aksi, string deskripsi)
        {
            var log = new RiwayatPerubahan
            {
                PerjanjianId = perjanjianId,
                Pengguna = pengguna, // Seharusnya didapat dari konteks autentikasi
                Aksi = aksi,
                DeskripsiPerubahan = deskripsi,
                Timestamp = DateTime.Now
            };
            _context.RiwayatPerubahan.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}