using System.ComponentModel.DataAnnotations;

namespace pdfquestAPI.Dtos
{
    // DTO untuk membuat entitas PenyediaLayanan baru
    public class CreatePenyediaLayananDto
    {
        [Required]
        public string NamaEntitasCalonProvider { get; set; }
        public string? JenisPerusahaan { get; set; }
        [Required]
        public string NoNibPihakKedua { get; set; }
        public string? AlamatPemegangPolis { get; set; }
        [Required]
        public string NamaPerwakilan { get; set; }
        public string? JabatanPerwakilan { get; set; }
        [Required]
        public string JenisFasilitas { get; set; }
        [Required]
        public string NamaFasilitasKesehatan { get; set; }
        public string? NamaDokumenIzin { get; set; }
        public string? NomorDokumenIzin { get; set; }
        public DateTime? TanggalDokumenIzin { get; set; }
        public string? InstansiPenerbitIzin { get; set; }
        [Required]
        public string NamaBank { get; set; }
        public string? NamaCabangBank { get; set; }
        [Required]
        public string NomorRekening { get; set; }
        [Required]
        public string PemilikRekening { get; set; }
        [Required]
        public string NamaPemilikNpwp { get; set; }
        [Required]
        public string NoNpwp { get; set; }
        [Required]
        public string JenisNpwp { get; set; }
    }
}
