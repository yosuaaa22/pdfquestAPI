using Microsoft.EntityFrameworkCore;
using pdfquestAPI.Models;

namespace pdfquestAPI.Data
{
    // EF Core DbContext untuk aplikasi — menyediakan akses ke tabel database melalui DbSet<T>.
    public class ApplicationDbContext : DbContext
    {
        // Dikonfigurasi melalui dependency injection; options diteruskan ke DbContext dasar.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Entitas domain: struktur dokumen / isi
        public DbSet<JudulIsi> JudulIsi { get; set; }
        public DbSet<PenyediaLayanan> PenyediaLayanan { get; set; }
        public DbSet<Perjanjian> Perjanjian { get; set; }
        public DbSet<PihakPertama> PihakPertama { get; set; }
        public DbSet<PoinKetentuanKhusus> PoinKetentuanKhusus { get; set; }
        public DbSet<SubBabKetentuanKhusus> SubBabKetentuanKhusus { get; set; }

        // Lampiran / data terkait file dan tindakan
        public DbSet<Lampiran_PIC> Lampiran_PIC { get; set; }
        public DbSet<Lampiran_TindakanMedis> Lampiran_TindakanMedis { get; set; }

        // Perubahan yang dilacak (change requests)
        public DbSet<ChangeRequest> ChangeRequests { get; set; }
        public DbSet<ChangeRequestItem> ChangeRequestItems { get; set; }
    }
}