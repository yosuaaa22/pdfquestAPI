using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pdfquestAPI.Models
{
    [Table("jdul_isi")]
    public class JudulIsi
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_jenis_penyedia")]
        public int IdJenisPenyedia { get; set; }

        [Column("judul_teks")]
        public string? JudulTeks { get; set; }

        [Column("urutan_tampil")]
        public int UrutanTampil { get; set; }
    }
}