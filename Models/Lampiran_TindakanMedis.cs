using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pdfquestAPI.Models
{
    [Table("Lampiran_TindakanMedis")]
    public class Lampiran_TindakanMedis
    {
        [Key]
        public int Id { get; set; }

        public int IdPenyedia { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? JenisTindakanMedis { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Tarif { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Keterangan { get; set; }
    }
}