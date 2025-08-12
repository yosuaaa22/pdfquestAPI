// In Models/PermintaanPersetujuan.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pdfquestAPI.Models
{
    [Table("PermintaanPersetujuan")]
    public class PermintaanPersetujuan
    {
        [Key]
        public int Id { get; set; }

        public int PerjanjianId { get; set; }

        [Required]
        public required byte[] PdfSnapshot { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Status { get; set; } // "Pending", "Approved", "Rejected"

        [Required]
        [MaxLength(255)]
        public required string DiajukanOleh { get; set; }

        public DateTime TanggalDiajukan { get; set; } = DateTime.Now;

        [MaxLength(255)]
        public string? DireviewOleh { get; set; }

        public DateTime? TanggalDireview { get; set; }

        public string? CatatanReview { get; set; }

        [ForeignKey("PerjanjianId")]
        public virtual Perjanjian Perjanjian { get; set; }
    }
}