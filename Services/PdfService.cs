using Microsoft.EntityFrameworkCore;
using pdfquestAPI.Documents;
using pdfquestAPI.Documents.Models;
using pdfquestAPI.Interfaces;
using QuestPDF.Fluent;
 
namespace pdfquestAPI.Services
{
    public class PdfService : IPdfService
    {
        private readonly IUnitOfWork _unitOfWork;
        // Kita butuh DbContext langsung untuk query yang lebih kompleks (Include)
        private readonly Data.ApplicationDbContext _context;
 
        public PdfService(IUnitOfWork unitOfWork, Data.ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
 
        public async Task<byte[]> GeneratePerjanjianPdfAsync(int perjanjianId)
        {
            // 1. Mengambil semua data yang dibutuhkan dalam satu query efisien
            var model = await BuildPerjanjianModelAsync(perjanjianId);
 
            // 2. Membuat dokumen QuestPDF dengan data yang sudah disiapkan
            var document = new PerjanjianDocument(model);
 
            // 3. Menghasilkan PDF menjadi byte array
            byte[] pdfBytes = document.GeneratePdf();
 
            return pdfBytes;
        }
 
        private async Task<PerjanjianDocumentModel> BuildPerjanjianModelAsync(int perjanjianId)
        {
            // Ambil data Perjanjian utama
            var perjanjian = await _unitOfWork.Perjanjian.GetByIdAsync(perjanjianId);
            if (perjanjian == null)
                throw new KeyNotFoundException("Perjanjian tidak ditemukan.");
 
            // Ambil data Pihak Kedua (Penyedia Layanan)
            var pihakKedua = await _unitOfWork.PenyediaLayanan.GetByIdAsync(perjanjian.IdPenyedia);
            if (pihakKedua == null)
                throw new KeyNotFoundException("Penyedia Layanan tidak ditemukan.");
 
            // Asumsi Pihak Pertama hanya ada satu, ambil yang pertama.
            // Jika ada relasi, sesuaikan query ini.
            var pihakPertama = (await _unitOfWork.PihakPertama.GetAllAsync()).FirstOrDefault();
            if (pihakPertama == null)
                throw new KeyNotFoundException("Data Pihak Pertama tidak ditemukan.");
 
            // Ambil data hierarkis untuk Ketentuan Khusus
            var ketentuanKhusus = await GetKetentuanKhususAsync();
 
            // Gabungkan semua data ke dalam model dokumen
            return new PerjanjianDocumentModel
            {
                Perjanjian = perjanjian,
                PihakPertama = pihakPertama,
                PihakKedua = pihakKedua,
                KetentuanKhusus = ketentuanKhusus
            };
        }
 
        private async Task<List<BabModel>> GetKetentuanKhususAsync()
        {
            // Ambil semua data dari DB
            var allBab = await _context.JudulIsi.OrderBy(j => j.UrutanTampil).ToListAsync();
            var allSubBab = await _context.SubBabKetentuanKhusus.OrderBy(s => s.UrutanTampil).ToListAsync();
            var allPoin = await _context.PoinKetentuanKhusus.OrderBy(p => p.UrutanTampil).ToListAsync();
 
            // Bangun struktur hierarkis di memori
            var babList = allBab.Select(bab => new BabModel
            {
                JudulTeks = bab.JudulTeks,
                UrutanTampil = bab.UrutanTampil,
                SubBab = allSubBab
                    .Where(sb => sb.IdJudul == bab.Id)
                    .Select(sb => new SubBabModel
                    {
                        Konten = sb.Konten,
                        UrutanTampil = sb.UrutanTampil,
                        Poin = BuildPoinTree(allPoin.Where(p => p.IdSubBab == sb.Id).ToList(), null)
                    }).ToList()
            }).ToList();
 
            return babList;
        }
 
        // Fungsi rekursif untuk membangun pohon poin (jika ada poin di dalam poin)
        private List<PoinModel> BuildPoinTree(List<pdfquestAPI.Models.PoinKetentuanKhusus> allPoin, int? parentId)
        {
            return allPoin
                .Where(p => p.Parent == parentId)
                .Select(p => new PoinModel
                {
                    Id = p.Id,
                    ParentId = p.Parent,
                    TeksPoin = p.TeksPoin,
                    UrutanTampil = p.UrutanTampil,
                    SubPoin = BuildPoinTree(allPoin, p.Id)
                }).ToList();
        }
    }
}