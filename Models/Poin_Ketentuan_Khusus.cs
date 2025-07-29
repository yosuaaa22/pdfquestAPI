using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pdfquestAPI.Models
{
    [Table("Poin_Ketentuan_Khusus")]
    public class PoinKetentuanKhusus
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_sub_bab")]
        public int IdSubBab { get; set; }

        [Column("teks_poin")]
        public string? TeksPoin { get; set; }

        [Column("parent")]
        public int? Parent { get; set; } // Menggunakan int? karena bisa NULL

        [Column("urutan_tampil")]
        public int UrutanTampil { get; set; }
    }
}