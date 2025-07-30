
namespace pdfquestAPI.Dtos;
using System.ComponentModel.DataAnnotations;

    public class CreatePenyediaLayananDto
    {
        [Required]
        public required  string NamaEntitasCalonProvider { get; set; }
        public string? JenisPerusahaan { get; set; }
        [Required]
        public required string NoNibPihakKedua { get; set; }
        public string? AlamatPemegangPolis { get; set; }
        [Required]
        public required  string NamaPerwakilan { get; set; }
        public string? JabatanPerwakilan { get; set; }
        [Required]
        public required  string JenisFasilitas { get; set; }
        [Required]
        public required  string NamaFasilitasKesehatan { get; set; }
        public string? NamaDokumenIzin { get; set; }
        public string? NomorDokumenIzin { get; set; }
        public DateTime? TanggalDokumenIzin { get; set; }
        public string? InstansiPenerbitIzin { get; set; }
        [Required]
        public required string NamaBank { get; set; }
        public string? NamaCabangBank { get; set; }
        [Required]
        public required  string NomorRekening { get; set; }
        [Required]
        public required string PemilikRekening { get; set; }
        [Required]
        public required string NamaPemilikNpwp { get; set; }
        [Required]
        public required string NoNpwp { get; set; }
        [Required]
        public required  string JenisNpwp { get; set; }
    }

