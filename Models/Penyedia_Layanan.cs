using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace pdfquestAPI.Models
{
    // Model untuk tabel Penyedia_Layanan
    [Table("Penyedia_Layanan")]
    public class PenyediaLayanan
    {
        [Key]
        [Column("id_penyedia")]
        public int IdPenyedia { get; set; }

        [Required]
        [Column("nama_Entitas_calon_provider")]
        public string NamaEntitasCalonProvider { get; set; }

        [Column("jenis_perusahaan")]
        public string? JenisPerusahaan { get; set; }

        [Required]
        [Column("no_nib_pihak_kedua")]
        public string NoNibPihakKedua { get; set; }

        [Column("alamat_pemegang_polis")]
        public string? AlamatPemegangPolis { get; set; }

        [Required]
        [Column("nama_perwakilan")]
        public string NamaPerwakilan { get; set; }

        [Column("jabatan_perwakilan")]
        public string? JabatanPerwakilan { get; set; }

        [Required]
        [Column("jenis_fasilitas")]
        public string JenisFasilitas { get; set; }

        [Required]
        [Column("nama_fasilitas_kesehatan")]
        public string NamaFasilitasKesehatan { get; set; }

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
        public string NamaBank { get; set; }

        [Column("nama_cabang_bank")]
        public string? NamaCabangBank { get; set; }

        [Required]
        [Column("Nomor_rekening")]
        public string NomorRekening { get; set; }

        [Required]
        [Column("pemilik_rekening")]
        public string PemilikRekening { get; set; }

        [Required]
        [Column("NAMA_PEMILIK_NPWP")]
        public string NamaPemilikNpwp { get; set; }

        [Required]
        [Column("NO_NPWP")]
        public string NoNpwp { get; set; }

        [Required]
        [Column("jenis_npwp")]
        public string JenisNpwp { get; set; }
    }
}