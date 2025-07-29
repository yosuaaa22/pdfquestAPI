using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pdfquestAPI.Models
{
    [Table("Sub_Bab_Ketentuan_Khusus")]
    public class SubBabKetentuanKhusus
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_judul")]
        public int IdJudul { get; set; }

        [Column("konten")]
        public string? Konten { get; set; }

        [Column("urutan_tampil")]
        public int UrutanTampil { get; set; }
    }
}