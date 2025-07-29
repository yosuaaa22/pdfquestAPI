using Microsoft.EntityFrameworkCore;
using pdfquestAPI.Models; // Pastikan namespace ini sesuai dengan lokasi Models Anda

namespace pdfquestAPI.Data
{
    // DbContext adalah jembatan antara aplikasi Anda dan database.
    // Kelas ini akan mengelola koneksi dan menerjemahkan query.
    public class ApplicationDbContext : DbContext
    {
        // Constructor ini penting untuk dependency injection.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Daftarkan semua model Anda di sini sebagai DbSet.
        // Setiap DbSet merepresentasikan satu tabel di database.
        public DbSet<JudulIsi> JudulIsi { get; set; }
        public DbSet<PenyediaLayanan> PenyediaLayanan { get; set; }
        public DbSet<Perjanjian> Perjanjian { get; set; }
        public DbSet<PihakPertama> PihakPertama { get; set; }
        public DbSet<PoinKetentuanKhusus> PoinKetentuanKhusus { get; set; }
        public DbSet<SubBabKetentuanKhusus> SubBabKetentuanKhusus { get; set; }
    }
}
