using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace pdfquestAPI.Models
{
    [Table("Penyedia_Layanan")]
    public class PenyediaLayanan
    {
        [Key]
        [Column("id_penyedia")]
        public int IdPenyedia { get; set; }

        [Required]
        [Column("nama_Entitas_calon_provider")]
        public required  string NamaEntitasCalonProvider { get; set; }

        [Column("jenis_perusahaan")]
        public string? JenisPerusahaan { get; set; }

        [Required]
        [Column("no_nib_pihak_kedua")]
        public required  string NoNibPihakKedua { get; set; }

        [Column("alamat_pemegang_polis")]
        public string? AlamatPemegangPolis { get; set; }

        [Required]
        [Column("nama_perwakilan")]
        public required  string NamaPerwakilan { get; set; }

        [Column("jabatan_perwakilan")]
        public string? JabatanPerwakilan { get; set; }

        [Required]
        [Column("jenis_fasilitas")]
        public required string JenisFasilitas { get; set; }

        [Required]
        [Column("nama_fasilitas_kesehatan")]
        public required string NamaFasilitasKesehatan { get; set; }

        [Column("nama_dokumen_izin")]
        public string? NamaDokumenIzin { get; set; }

        [Column("nomor_dokumen_izin")]
        public string? NomorDokumenIzin { get; set; }

        [Column("tanggal_dokumen_izin")]
        public DateTime? TanggalDokumenIzin { get; set; }

        [Column("instansi_penerbit_izin")]
        public string? InstansiPenerbitIzin { get; set; }

        [Required]
        [Column("Nama_Bank")]
        public required  string NamaBank { get; set; }

        [Column("nama_cabang_bank")]
        public string? NamaCabangBank { get; set; }

        [Required]
        [Column("Nomor_rekening")]
        public required string NomorRekening { get; set; }

        [Required]
        [Column("pemilik_rekening")]
        public required  string PemilikRekening { get; set; }

        [Required]
        [Column("NAMA_PEMILIK_NPWP")]
        public required string NamaPemilikNpwp { get; set; }

        [Required]
        [Column("NO_NPWP")]
        public required  string NoNpwp { get; set; }

        [Required]
        [Column("jenis_npwp")]
        public required  string JenisNpwp { get; set; }
    }
}