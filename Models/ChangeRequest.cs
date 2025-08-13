using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pdfquestAPI.Models
{
    [Table("ChangeRequests")]
    public class ChangeRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PerjanjianId { get; set; }

        [Required]
        public DateTime TanggalRequest { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(255)]
        public required string DiajukanOleh { get; set; } // e.g., "Provider_ABC" atau "Inhealth_User"

        public string? Deskripsi { get; set; } // Deskripsi umum request, misal: "Revisi Bab 3"

        [Required]
        [MaxLength(50)]
        public required string Status { get; set; } // "Pending", "Approved", "Rejected"

        public DateTime? TanggalDiputuskan { get; set; }

        [MaxLength(255)]
        public string? DiputuskanOleh { get; set; }

        // Navigation Properties
        [ForeignKey("PerjanjianId")]
        public virtual Perjanjian Perjanjian { get; set; }

        public virtual ICollection<ChangeRequestItem> Items { get; set; } = new List<ChangeRequestItem>();
    }
}