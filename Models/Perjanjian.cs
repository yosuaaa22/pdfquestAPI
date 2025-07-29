// In Models/Perjanjian.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pdfquestAPI.Models
{
    [Table("Perjanjian")]
    public class Perjanjian
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_perjanjian")]
        public int IdPerjanjian { get; set; }

        [Column("id_penyedia")]
        public int IdPenyedia { get; set; }

        [Column("no_pt_inhealth")]
        public string? NoPtInhealth { get; set; }

        [Column("no_pt_pihak_kedua")]
        public string? NoPtPihakKedua { get; set; }

        [Column("tanggal_tanda_tangan")]
        public DateTime TanggalTandaTangan { get; set; }

        [Column("nomor_berita_acara")]
        public string? NomorBeritaAcara { get; set; }

        [Column("tanggal_berita_acara")]
        public DateTime TanggalBeritaAcara { get; set; }

        [Column("jangka_waktu_perjanjian")]
        public string? JangkaWaktuPerjanjian { get; set; }
    }
}