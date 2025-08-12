// In Models/RiwayatPerubahan.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pdfquestAPI.Models
{
    [Table("RiwayatPerubahan")]
    public class RiwayatPerubahan
    {
        [Key]
        public int Id { get; set; }

        public int PerjanjianId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(255)]
        public required string Pengguna { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Aksi { get; set; }

        [Required]
        public required string DeskripsiPerubahan { get; set; }

        [ForeignKey("PerjanjianId")]
        public virtual Perjanjian Perjanjian { get; set; }
    }
}