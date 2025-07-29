// In Models/DTOs/PerjanjianDto.cs
namespace pdfquestAPI.Models.DTOs
{
    public class PerjanjianDto
    {
        // Data dari tabel Perjanjian
        public int IdPerjanjian { get; set; }
        public string? NoPtInhealth { get; set; }
        public string? NoPtPihakKedua { get; set; }
        public DateTime TanggalTandaTangan { get; set; }
        public string? NomorBeritaAcara { get; set; }

        // Data gabungan yang sudah siap pakai
        public List<JudulIsiDto> DaftarIsi { get; set; } = new();
    }
}