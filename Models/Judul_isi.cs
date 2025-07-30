// Di dalam file: Models/JudulIsi.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pdfquestAPI.Models
{
    [Table("judul_isi")]
    public class JudulIsi
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // GANTI NAMA PROPERTI INI
        [Column("id_penyedia")] 
        public int? IdPenyedia { get; set; } 

        [Column("judul_teks")]
        public string? JudulTeks { get; set; }

        [Column("urutan_tampil")]
        public int UrutanTampil { get; set; }

        // OPSIONAL TAPI SANGAT DIREKOMENDASIKAN: Tambahkan Navigation Property
        [ForeignKey("IdPenyedia")]
        public virtual PenyediaLayanan? PenyediaLayanan { get; set; }
    }
}