using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pdfquestAPI.Models
{
    // Atribut [Table] ini secara eksplisit memberitahu EF Core
    // bahwa kelas ini terhubung ke tabel bernama "Lampiran_PIC" di database.
    [Table("Lampiran_PIC")]
    public class Lampiran_PIC
    {
        [Key] // Menandakan ini adalah Primary Key
        public int Id { get; set; }

        public int IdPenyedia { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public required string JenisPIC { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string? NamaPIC { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? NomorTelepon { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string? AlamatEmail { get; set; }
    }
}